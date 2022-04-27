using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SpaceInsurgency;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency.Inventory
{
	[AdvancedInspector]
	public class ShipsContent : SharedCode.Behaviours.Base
	{
		#region Event Definitions

		#endregion Event Definitions
		#region Variables

		[SerializeField]
		Transform m_ItemParentTransform;

		[Inspect, Group("Editor")]
		public Transform ItemParentTransform
		{
			get { return m_ItemParentTransform; }
			set { m_ItemParentTransform = value; }
		}

		[SerializeField]
		GameObject m_ItemPrefab;

		[Inspect, Group("Editor"), DontAllowSceneObject]
		public GameObject ItemPrefab
		{
			get { return m_ItemPrefab; }
			set { m_ItemPrefab = value; }
		}

		ShipsContentItem m_SelectedItem;
		[Inspect, Group("Runtime")]
		public ShipsContentItem SelectedItem
		{
			get { return m_SelectedItem; }
			set
			{
				if (null != m_SelectedItem)
				{
					m_SelectedItem.IsSelected = false;

					m_EnergyDisplay.EquippableComponent = null;
					m_EnginesDisplay.EquippableComponent = null;
					m_HealthDisplay.EquippableComponent = null;
					m_SensorsDisplay.EquippableComponent = null;
					m_ShieldDisplay.EquippableComponent = null;
					m_WeaponsDisplay.EquippableComponent = null;
					m_CargoDisplay.EquippableComponent = null;
				}



				m_SelectedItem = value;

				if (null != m_SelectedItem)
				{
					m_SelectedItem.IsSelected = true;

					DynamicAgent agent = m_SelectedItem.DynamicAgent;
					Assert.IsNotNull<DynamicAgent>(agent);

					m_EnergyDisplay.EquippableComponent = agent.GetComponent<Energy>();
					m_EnginesDisplay.EquippableComponent = agent.GetComponent<Engines>();
					m_HealthDisplay.EquippableComponent = agent.GetComponent<Health>();
					m_SensorsDisplay.EquippableComponent = agent.GetComponent<Sensors>();
					m_ShieldDisplay.EquippableComponent = agent.GetComponent<Shield>();
					m_WeaponsDisplay.EquippableComponent = agent.GetComponent<Weapons>();
					m_CargoDisplay.EquippableComponent = agent.GetComponent<Cargo>();
				}
			}
		}

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_EnergyDisplay;

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_EnginesDisplay;

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_HealthDisplay;

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_SensorsDisplay;

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_ShieldDisplay;

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_WeaponsDisplay;

		[Inspect, Group("Editor")]
		public EquippableComponentDisplay m_CargoDisplay;

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

		}

		protected override void Start()
		{
			base.Start();

			SubSignal();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubSignal();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;

			Membership.OnInstanceAdded += Membership_OnInstanceAdded;
			Membership.OnInstanceRemoved += Membership_OnInstanceRemoved;
			Membership.OnFactionChanged += Membership_OnFactionChanged;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;

			Membership.OnInstanceAdded -= Membership_OnInstanceAdded;
			Membership.OnInstanceRemoved -= Membership_OnInstanceRemoved;
			Membership.OnFactionChanged -= Membership_OnFactionChanged;

			signalSub = false;
		}

		#endregion Setup
		#region Events

		void Membership_OnInstanceAdded(Membership membership)
		{
			if (membership.Faction != Faction.PlayerFaction)
				return;

			Relay();
		}

		void Membership_OnInstanceRemoved(Membership membership)
		{
			if (membership.Faction != Faction.PlayerFaction)
				return;

			Relay();
		}

		void Membership_OnFactionChanged(Membership membership, Faction oldFaction, Faction newFaction)
		{
			if (oldFaction == Faction.PlayerFaction)
				Relay();
			else if (newFaction == Faction.PlayerFaction)
				Relay();
		}

		#endregion Events
		#region Main

		[Inspect, Group("Runtime")]
		public void Relay()
		{
			int childCount = m_ItemParentTransform.transform.childCount;

			// Remove all children.
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < childCount; i++)
				children.Add(m_ItemParentTransform.transform.GetChild(i));
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].GetComponent<ShipsContentItem>() == SelectedItem)
					SelectedItem = null;
				GameObject.Destroy(children[i].gameObject);
			}

			if (null == Faction.PlayerFaction)
				return;

			List<DynamicAgent> agents = Faction.PlayerFaction.DynamicAgents;
			for (int i=0; i<agents.Count; i++)
			{
				GameObject itemObj = GameObject.Instantiate<GameObject>(m_ItemPrefab);
				ShipsContentItem shipsContentItem = itemObj.GetComponent<ShipsContentItem>();
				shipsContentItem.ShipsContent = this;
				shipsContentItem.DynamicAgent = agents[i];
				itemObj.transform.SetParent(m_ItemParentTransform.transform, false);
			}

		}

		#endregion Main
	}
}