using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Sensors : EquippableComponent
	{
		#region Event Definitions

		public event Action<Sensors, SpaceObject> OnTargetChanged; // Sensors source, SpaceObject newTarget
		public event Action<Sensors, float> OnSensorRangeChanged; // Sensors source, float newRange

		#endregion Event Definitions
		#region Variables

		SpaceObject m_Target = null;
		[Inspect, Group("Runtime"), ReadOnly]
		public SpaceObject Target
		{
			get
			{
				return m_Target;
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				if (null != OnTargetChanged)
					OnTargetChanged(this, value);
			}
		}

		[Inspect, Group("Runtime")]
		void ClearCurrentTarget()
		{
			Target = null;
		}

		[SerializeField]
		float m_MinSensorRange = 1f;

		[Inspect, ReadOnly, Group("Editor")]
		public float MinSensorRange
		{
			get { return m_MinSensorRange; }
			set { m_MinSensorRange = value; }
		}

		[SerializeField]
		float m_MaxSensorRange = 100f;

		[Inspect, ReadOnly, Group("Editor")]
		public float MaxSensorRange
		{
			get { return m_MaxSensorRange; }
			set { m_MaxSensorRange = value; }
		}

		float m_SensorRangeCurrent;

		[Inspect, Group("Runtime")]
		public float SensorRangeCurrent
		{
			get
			{
				if (true == GetComponent<Health>().IsDestroyed)
					return 0f;

				return m_SensorRangeCurrent.Clamp<float>(m_MinSensorRange, m_MaxSensorRange);
			}
			set
			{
				OnSensorRangeChanged(this, value);
			}
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			OnTargetChanged = new Action<Sensors, SpaceObject>(DefaultOnTargetChanged);
			OnSensorRangeChanged = new Action<Sensors, float>(DefaultOnSensorRangeChanged);
			OnFinishedSpawnSetup += delegate {
				RecalculateSensorRangeCurrent();
				OnEnable();
			};
		}

		protected override void Start()
		{
			base.Start();

			// Update the sensor ranges when items are changed.
			OnAddedItem += delegate {
				RecalculateSensorRangeCurrent();
			};
			OnRemovedItem += delegate {
				Debug.Log("RecalculateSensorRangeCurrent()");
				RecalculateSensorRangeCurrent();
			};

			SubscribeToEvents();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

#warning have this for ships your mouse hovers over.
			if (null == sensorCircle)
				SetupSensorCircle();

			SubscribeToEvents();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (null != sensorCircle)
				DeleteSensorCircle();

			UnsubscribeToEvents();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnsubscribeToEvents();
		}

		bool eventsSubscribed = false;
		void SubscribeToEvents()
		{
			if (true == eventsSubscribed)
				return;
			if (null == FTLManager.Main)
				return;

			FTLManager.Main.OnFTLTransitonBegin += FTLManager_OnFTLTransitonBegin;
			FTLManager.Main.OnFTLTransitonEnd += FTLManager_OnFTLTransitonEnd;

			eventsSubscribed = true;
		}

		void UnsubscribeToEvents()
		{
			if (false == eventsSubscribed)
				return;
			if (null == FTLManager.Main)
				return;

			FTLManager.Main.OnFTLTransitonBegin -= FTLManager_OnFTLTransitonBegin;
			FTLManager.Main.OnFTLTransitonEnd -= FTLManager_OnFTLTransitonEnd;

			eventsSubscribed = false;
		}
		

		#endregion Setup
		#region Events

		void FTLManager_OnFTLTransitonBegin(FTLManager sender)
		{
			DeleteSensorCircle();
		}

		void FTLManager_OnFTLTransitonEnd(FTLManager sender)
		{
			SetupSensorCircle();
		}

		#endregion Events
		#region Sensor Range Circle
#warning actually have this hide objects

		VectorLine sensorCircle;

		[Inspect, Group("Utility")]
		private void SetupSensorCircle()
		{
			DeleteSensorCircle();

			if (GetComponent<Membership>().Faction != Faction.PlayerFaction)
				return;
			// We don't show the censor circle on the galaxy map.
			if (true == LevelGeometry.Main.IsGalaxyMap)
				return;

			Vector3 forward = new Vector3(0, 0, SensorRangeCurrent);

			Vector3[] points = new Vector3[90];
			for (int i = 0, angle = 0; i < 90; i++, angle += 4)
				points[i] = forward.RotateAroundPivot(Vector3.zero, Quaternion.Euler(0, angle, 0));

			if (null == sensorCircle)
			{
				sensorCircle = new VectorLine("Sensor Line", points, null, 4f, LineType.Continuous, Joins.Fill);
				sensorCircle.color = new Color(1f, 1f, 1f, 0.05f);
				sensorCircle.drawTransform = transform;
				sensorCircle.MakeSpline(points, true);
				sensorCircle.Draw3DAuto();
			}
		}

		[Inspect, Group("Utility")]
		public void DeleteSensorCircle()
		{
			if (null != sensorCircle)
			{
				VectorLine.Destroy(ref sensorCircle);
				sensorCircle = null;
			}
		}

		#endregion Sensor Range Circle
		#region Main

		void RecalculateSensorRangeCurrent()
		{
			float sensorRange = 0f;
			if (null != GetComponent<DynamicAgent>())
				sensorRange += GetComponent<DynamicAgent>().AgentDefinition.DefaultSensorRange;

			for (int i = 0; i < Items.Count; i++)
			{
				Base item = Items[i].ItemDefinition;
				sensorRange += item.MaxSensorRangeModifier;
			}

			SensorRangeCurrent = sensorRange;
		}

		#endregion Main
		#region Events

		void DefaultOnSensorRangeChanged(Sensors source, float newRange)
		{
			m_SensorRangeCurrent = newRange.Clamp<float>(m_MinSensorRange, m_MaxSensorRange);

			if (null != sensorCircle)
				DeleteSensorCircle();
			if (null == sensorCircle)
				SetupSensorCircle();
		}

		void DefaultOnTargetChanged(Sensors source, SpaceObject newTarget)
		{
			// Unsubscribe from old destroy event.
			if (null != m_Target && null != m_Target.GetComponent<Health>())
				m_Target.GetComponent<Health>().OnShipDestroyed -= OnShipDestroyed;

			m_Target = newTarget;

			// Subscribe to the destroy event of the new object.
			if (null != m_Target && null != m_Target.GetComponent<Health>())
				m_Target.GetComponent<Health>().OnShipDestroyed += OnShipDestroyed;
		}

		void OnShipDestroyed(Health source)
		{
			Target = null;
		}

		#endregion Targeting
		#region Anccessors

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;

			return newItem.CanBeAddedToSensors ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
		}

		#endregion Anccessors
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();
			public string target = null;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);
			if (null != m_Target)
				serialized.target = m_Target.UniqueName;

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			for (int i = 0; i < serialized.items.Count; i++)
				AddItem(Base.GetFromUniqueName(serialized.items[i]));
		}

		#endregion Serialization
	}




















































}