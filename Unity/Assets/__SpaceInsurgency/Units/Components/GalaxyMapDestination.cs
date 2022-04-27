using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;


namespace SpaceInsurgency
{
	public class GalaxyMapDestination : SharedCode.Behaviours.InstanceTracked<GalaxyMapDestination>
	{
		#region Variables

		[Inspect]
		[Restrict("ListOfPlanetarySystemsForEditor")]
		public string system;
		public List<string> ListOfPlanetarySystemsForEditor()
		{
			return PlanetarySystem.kListOfPlanetarySystemsForEditor;
		}

		[SerializeField]
		TriggerDetectWithin m_TriggerObject;
		[Inspect]
		public TriggerDetectWithin TriggerObject
		{
			get { return m_TriggerObject; }
			set { m_TriggerObject = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			UniqueName = system;
		}

		protected override void Start()
		{
			base.Start();

			SubscribeToEvents();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubscribeToEvents();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

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

			m_TriggerObject.OnObjectAdded += TriggerDetectWithin_OnObjectAdded;
			m_TriggerObject.OnObjectRemoved += TriggerDetectWithin_OnObjectRemoved;

			eventsSubscribed = true;
		}

		void UnsubscribeToEvents()
		{
			if (false == eventsSubscribed)
				return;

			m_TriggerObject.OnObjectAdded -= TriggerDetectWithin_OnObjectAdded;
			m_TriggerObject.OnObjectRemoved -= TriggerDetectWithin_OnObjectRemoved;

			eventsSubscribed = false;
		}

		#endregion Setup
		#region Events

		void TriggerDetectWithin_OnObjectAdded(SpaceObject obj)
		{
			ObjectsChanged();
		}

		void TriggerDetectWithin_OnObjectRemoved(SpaceObject obj)
		{
			ObjectsChanged();
		}

		void ObjectsChanged()
		{
			Debug.Log("ObjectsChanged()");
			int numObj = m_TriggerObject.ObjectsInArea.Count;
			if (numObj == 0)
			{
				if (this == TempEnterSystem.First.Focus)
					TempEnterSystem.First.Focus = null;
			}
			else
			{
				TempEnterSystem.First.Focus = this;
			}
		}

		#endregion Events
		#region Main





		#endregion Main
	}
}
