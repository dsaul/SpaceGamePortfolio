using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SpaceInsurgency;
using AdvancedInspector;
using SharedCode;
using Vectrosity;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public sealed class DynamicAgent : SharedCode.Behaviours.InstanceTracked<DynamicAgent>
	{
		#region Variables

		DynamicAgentDefinition m_AgentDefinition;

		[Inspect, ReadOnly, Group("Runtime")]
		public DynamicAgentDefinition AgentDefinition
		{
			get { return m_AgentDefinition; }
			set { m_AgentDefinition = value; }
		}

		GameObject m_ShipBodyObject;

		[Inspect, ReadOnly, Group("Runtime")]
		public GameObject VisualObject3D
		{
			get { return m_ShipBodyObject; }
			set { m_ShipBodyObject = value; }
		}

		Transform m_ShipShieldObject;

		[Inspect, ReadOnly, Group("Runtime")]
		public Transform ShipShieldObject
		{
			get { return m_ShipShieldObject; }
			set { m_ShipShieldObject = value; }
		}

		ShipModelParts m_Parts;
		[Inspect, ReadOnly, Group("Runtime")]

		public ShipModelParts Parts
		{
			get { return m_Parts; }
			set
			{ 
				m_Parts = value;
			}
		}

		
		[SerializeField]
		string m_GivenName;

		[Inspect, Group("Editor")]
		public string GivenName
		{
			get { return m_GivenName; }
			set { OnGivenNameChanged(this,value); }
		}
		Action<DynamicAgent, string> _OnGivenNameChanged;
		public Action<DynamicAgent, string> OnGivenNameChanged // DynamicAgent sender, string value
		{
			get
			{
				if (null == _OnGivenNameChanged)
					_OnGivenNameChanged = new Action<DynamicAgent, string>(delegate(DynamicAgent sender, string value) { m_GivenName = value; });
				return _OnGivenNameChanged;
			}
			set { _OnGivenNameChanged = value; }
		}

		[SerializeField]
		GameObject m_AnimationContainer;

		[Inspect, Group("Editor")]
		public GameObject AnimationContainer
		{
			get { return m_AnimationContainer; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			OnFinishedSpawnSetup += DynamicAgent_OnFinishedSpawnSetup;
		}

		protected override void Start()
		{
			base.Start();

			if (true == string.IsNullOrEmpty(m_GivenName))
			{
				// No name specified, we must genereate one.

				string firstName = UnityEngine.Random.value < 0.5f ? AssetManager.Main.NamesMale[UnityEngine.Random.Range(0, AssetManager.Main.NamesMale.Length)] : AssetManager.Main.NamesFemale[UnityEngine.Random.Range(0, AssetManager.Main.NamesFemale.Length)];
				string lastName = AssetManager.Main.NamesLast[UnityEngine.Random.Range(0, AssetManager.Main.NamesLast.Length)];
				GivenName = string.Format("ISF {0} {1}", firstName, lastName);
			}
		}
		
		
		protected override void OnEnable()
		{
			base.OnEnable();

			EnsureComponents();

			if (Application.isEditor)
			{
				SetTimer(1f, true, UpdateEditorName);
			}

			SubSignal();
		}

		void DynamicAgent_OnFinishedSpawnSetup(SharedCode.Behaviours.Base source)
		{
			Cargo cargo = GetComponent<Cargo>();
			Sensors sensors = GetComponent<Sensors>();
			Communication communication = GetComponent<Communication>();
			Engines engines = GetComponent<Engines>();
			Energy energy = GetComponent<Energy>();
			Health health = GetComponent<Health>();
			Shield shield = GetComponent<Shield>();
			Weapons weapons = GetComponent<Weapons>();
			Membership membership = GetComponent<Membership>();
			FTL ftl = GetComponent<FTL>();
			SpaceObject spaceObject = GetComponent<SpaceObject>();
			AI ai = GetComponent<AI>();
			Trader trader = GetComponent<Trader>();

			if (null != cargo)
				cargo.OnFinishedSpawnSetup(cargo);
			if (null != sensors)
				sensors.OnFinishedSpawnSetup(sensors);
			if (null != communication)
				communication.OnFinishedSpawnSetup(communication);
			if (null != engines)
				engines.OnFinishedSpawnSetup(engines);
			if (null != energy)
				energy.OnFinishedSpawnSetup(energy);
			if (null != health)
				health.OnFinishedSpawnSetup(health);
			if (null != shield)
				shield.OnFinishedSpawnSetup(shield);
			if (null != weapons)
				weapons.OnFinishedSpawnSetup(weapons);
			if (null != membership)
				membership.OnFinishedSpawnSetup(membership);
			if (null != ftl)
				ftl.OnFinishedSpawnSetup(ftl);
			if (null != spaceObject)
				spaceObject.OnFinishedSpawnSetup(spaceObject);
			if (null != ai)
				ai.OnFinishedSpawnSetup(ai);
			if (null != trader)
				trader.OnFinishedSpawnSetup(trader);
		}

		void EnsureComponents()
		{
			if (null == m_AnimationContainer)
			{
				m_AnimationContainer = new GameObject("Animation Container");
				m_AnimationContainer.AddComponent<SpaceObject>();

				m_AnimationContainer.transform.parent = transform;
				m_AnimationContainer.transform.localPosition = Vector3.zero;
				m_AnimationContainer.transform.localRotation = Quaternion.identity;
				m_AnimationContainer.transform.localScale = Vector3.one;
			}
			
			if (null == GetComponent<Rigidbody>())
			{
				Rigidbody rb = gameObject.AddComponent<Rigidbody>();
				rb.mass = 1f;
				rb.drag = 0f;
				rb.angularDrag = 0.05f;
				rb.useGravity = false;
				rb.isKinematic = true;
				rb.interpolation = RigidbodyInterpolation.None;
				rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}

			if (null == GetComponent<Membership>())
				gameObject.AddComponent<Membership>();
			if (null == GetComponent<Communication>())
				gameObject.AddComponent<Communication>();
			if (null == GetComponent<Sensors>())
				gameObject.AddComponent<Sensors>();
			if (null == GetComponent<FTL>())
				gameObject.AddComponent<FTL>();

			if (null == GetComponent<SpaceObject>())
			{
				SpaceObject s = gameObject.AddComponent<SpaceObject>();
				s.ForceRoot = true;
			}

			if (null == GetComponent<Weapons>())
				gameObject.AddComponent<Weapons>();
			if (null == GetComponent<Shield>())
				gameObject.AddComponent<Shield>();
			if (null == GetComponent<Health>())
				gameObject.AddComponent<Health>();
			if (null == GetComponent<Energy>())
				gameObject.AddComponent<Energy>();
			if (null == GetComponent<Cargo>())
				gameObject.AddComponent<Cargo>();
			if (null == GetComponent<Engines>())
				gameObject.AddComponent<Engines>();
			if (null == GetComponent<AI>())
				gameObject.AddComponent<AI>();
			if (null == GetComponent<Trader>())
				gameObject.AddComponent<Trader>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(UpdateEditorName);

			UnSubSignal();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;
			if (null == SaveStateManager.First)
				return;

			SaveStateManager.First.OnBeforeSave += DynamicAgent_OnBeforeSave;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == SaveStateManager.First)
				return;

			SaveStateManager.First.OnBeforeSave -= DynamicAgent_OnBeforeSave;

			signalSub = false;
		}

		public void CleanRemove()
		{
			m_AgentDefinition.BodyPrefabPool.Abandon(VisualObject3D);

			Destroy(gameObject);
		}

		#endregion Setup
		#region Utility

		public static void CentreCameraOnShips(List<DynamicAgent> ships)
		{
			Vector3 cameraPoint = Vector3.zero;

			for (int i = 0; i < ships.Count; i++)
			{
				DynamicAgent ship = ships[i];
				if (Vector3.zero == cameraPoint)
					cameraPoint = ship.transform.position;
				else
					cameraPoint = Vector3.Lerp(cameraPoint, ship.transform.position, 0.5f);
			}
#warning we need this to be facing thet ship from the rear
			CameraManager.Main.CameraGame.GetComponent<RtsCamera>().JumpTo(cameraPoint, true);
		}

		[Inspect, Group("Utility")]
		void Select()
		{
			SelectionManager.Main.DeSelectAll();
			SelectionManager.Main.Select(GetComponent<SpaceObject>());
		}

		[Inspect, Group("Utility")]
		void Follow()
		{
			CameraManager.Main.CameraGame.GetComponent<RtsCamera>().Follow(transform);
		}

		[Inspect, Group("Utility")]
		void SelectAndFollow()
		{
			Select();
			Follow();
		}

		void UpdateEditorName()
		{
			gameObject.name = string.Format("% {0} % {1}", GetComponent<Membership>().Faction.DisplayName, AgentDefinition.DisplayName);
		}

		#endregion Utility
		#region Serialization

		void DynamicAgent_OnBeforeSave(SaveStateManager sender)
		{
			sender.SaveState.dynamicAgents.Add(GetSerialized());
		}

		[Serializable]
		public class Serialized
		{
			public string dynamicAgentDefinition = null;
			public string givenName = null;
			
			// transform
			public Vector3 position = Vector3.zero;
			public Quaternion rotation = Quaternion.identity;

			public Membership.Serialized membership = new Membership.Serialized();
			public Sensors.Serialized sensors = new Sensors.Serialized();
			public Communication.Serialized communication = new Communication.Serialized();
			public Engines.Serialized engines = new Engines.Serialized();
			public FTL.Serialized ftl = new FTL.Serialized();
			public SpaceObject.Serialized spaceObject = new SpaceObject.Serialized();
			public Weapons.Serialized weapons = new Weapons.Serialized();
			public Energy.Serialized energy = new Energy.Serialized();
			public Shield.Serialized shield = new Shield.Serialized();
			public Health.Serialized health = new Health.Serialized();
			public Cargo.Serialized cargo = new Cargo.Serialized();
			public AI.Serialized ai = new AI.Serialized();
			public Trader.Serialized trader = new Trader.Serialized();

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			// transform
			serialized.position = transform.position;
			serialized.rotation = transform.rotation;

			// dynamic agent
			serialized.dynamicAgentDefinition = AgentDefinition.UniqueName;
			serialized.givenName = GivenName;


			Membership membership = GetComponent<Membership>();
			Cargo cargo = GetComponent<Cargo>();
			Sensors sensors = GetComponent<Sensors>();
			Communication communication = GetComponent<Communication>();
			Engines engines = GetComponent<Engines>();
			Energy energy = GetComponent<Energy>();
			Health health = GetComponent<Health>();
			Shield shield = GetComponent<Shield>();
			Weapons weapons = GetComponent<Weapons>();
			FTL ftl = GetComponent<FTL>();
			SpaceObject spaceObject = GetComponent<SpaceObject>();
			AI ai = GetComponent<AI>();
			Trader trader = GetComponent<Trader>();

			if (null != membership)
				serialized.membership = membership.GetSerialized();
			if (null != cargo)
				serialized.cargo = cargo.GetSerialized();
			if (null != sensors)
				serialized.sensors = sensors.GetSerialized();
			if (null != communication)
				serialized.communication = communication.GetSerialized();
			if (null != engines)
				serialized.engines = engines.GetSerialized();
			if (null != energy)
				serialized.energy = energy.GetSerialized();
			if (null != health)
				serialized.health = health.GetSerialized();
			if (null != shield)
				serialized.shield = shield.GetSerialized();
			if (null != weapons)
				serialized.weapons = weapons.GetSerialized();

			if (null != ftl)
				serialized.ftl = ftl.GetSerialized();
			if (null != spaceObject)
				serialized.spaceObject = spaceObject.GetSerialized();
			if (null != ai)
				serialized.ai = ai.GetSerialized();
			if (null != trader)
				serialized.trader = trader.GetSerialized();

			
			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			EnsureComponents();

			DynamicAgentDefinition dynamicAgentDefinition = DynamicAgentDefinition.GetFromUniqueName(serialized.dynamicAgentDefinition);
			Assert.IsNotNull<DynamicAgentDefinition>(dynamicAgentDefinition);

			AgentDefinition = dynamicAgentDefinition;
			GivenName = serialized.givenName;

			transform.parent = null;
			transform.position = serialized.position;
			transform.rotation = serialized.rotation;

			Membership membership = GetComponent<Membership>();
			Cargo cargo = GetComponent<Cargo>();
			Sensors sensors = GetComponent<Sensors>();
			Communication communication = GetComponent<Communication>();
			Engines engines = GetComponent<Engines>();
			Energy energy = GetComponent<Energy>();
			Health health = GetComponent<Health>();
			Shield shield = GetComponent<Shield>();
			Weapons shipWeapons = GetComponent<Weapons>();
			FTL ftl = GetComponent<FTL>();
			SpaceObject spaceObject = GetComponent<SpaceObject>();
			AI ai = GetComponent<AI>();
			Trader trader = GetComponent<Trader>();

			if (null != membership && null != serialized.membership)
				membership.RestoreSerialized(serialized.membership);
			if (null != cargo && null != serialized.cargo)
				cargo.RestoreSerialized(serialized.cargo);
			if (null != sensors && null != serialized.sensors)
				sensors.RestoreSerialized(serialized.sensors);
			if (null != communication && null != serialized.communication)
				communication.RestoreSerialized(serialized.communication);
			if (null != engines && null != serialized.engines)
				engines.RestoreSerialized(serialized.engines);
			if (null != energy && null != serialized.energy)
				energy.RestoreSerialized(serialized.energy);
			if (null != health && null != serialized.health)
				health.RestoreSerialized(serialized.health);
			if (null != shield && null != serialized.shield)
				shield.RestoreSerialized(serialized.shield);
			if (null != shipWeapons && null != serialized.weapons)
				shipWeapons.RestoreSerialized(serialized.weapons);
			if (null != ftl && null != serialized.ftl)
				ftl.RestoreSerialized(serialized.ftl);
			if (null != spaceObject && null != serialized.spaceObject)
				spaceObject.RestoreSerialized(serialized.spaceObject);
			if (null != ai && null != serialized.ai)
				ai.RestoreSerialized(serialized.ai);
			if (null != trader && null != serialized.trader)
				trader.RestoreSerialized(serialized.trader);

		}

		#endregion Serialization
	}
}