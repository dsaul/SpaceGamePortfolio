using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using Vectrosity;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Energy : EquippableComponent
	{
		#region Event Definitions

		public event Action<Energy, float> OnEnergyChanged; // Energy source, float newEnergy

		#endregion Event Definitions
		#region Variables

		float m_EnergyCurrent = 100f;
		[Inspect, Group("Runtime")]
		public float EnergyCurrent
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				if (null != GetComponent<Health>() && true == GetComponent<Health>().IsDestroyed)
					return 0f;
				else
					return m_EnergyCurrent.Clamp<float>(0, m_EnergyMax);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				if (null != OnEnergyChanged)
					OnEnergyChanged(this, value);
			}
		}
		void DefaultOnEnergyChanged(Energy source, float newEnergy)
		{
			m_EnergyCurrent = newEnergy.Clamp<float>(0, m_EnergyMax);
		}

		float m_EnergyMax;
		[Inspect, Group("Runtime"), ReadOnly]
		public float EnergyMax
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return m_EnergyMax;
			}
		}
		void RecalculateEnergyMax()
		{
			if (false == Application.isPlaying)
			{
				m_EnergyMax = float.NaN;
				return;
			}

			float r = GetComponent<DynamicAgent>().AgentDefinition.EnergyMaxBase;

			for (int i = 0; i < Items.Count; i++)
				r += Items[i].ItemDefinition.MaxEnergyModifier;
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
				r += GlobalItemsManager.Main.GlobalItems[i].MaxEnergyModifier;

			m_EnergyMax = r;
		}

		float m_EnergyRechargeEachSecond;
		[Inspect, Group("Runtime"), ReadOnly]
		public float EnergyRechargeEachSecond
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return m_EnergyRechargeEachSecond;
			}
		}
		void RecalculateEnergyRechargeEachSecond()
		{
			if (false == Application.isPlaying)
			{
				m_EnergyRechargeEachSecond = float.NaN;
				return;
			}

			float r = GetComponent<DynamicAgent>().AgentDefinition.EnergyRechargeBase;

			for (int i = 0; i < Items.Count; i++)
				r += Items[i].ItemDefinition.EnergyRechargeModifier;
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
				r += GlobalItemsManager.Main.GlobalItems[i].EnergyRechargeModifier;

			m_EnergyRechargeEachSecond = r;
		}

		List<float> m_EnergyCostsMultipliers;
		[Inspect, Group("Runtime"), ReadOnly]
		public List<float> EnergyCostsMultipliers
		{
			get { return m_EnergyCostsMultipliers; }
		}
		void RecalculateEnergyCostsMultipliers()
		{
			m_EnergyCostsMultipliers.Clear();

			for (int i = 0; i < Items.Count; i++)
			{
				Base item = Items[i].ItemDefinition;
				Assert.AreNotEqual<float>(0, item.EnergyCostsMultiplier);
				if (item.EnergyCostsMultiplier == 1)
					continue;
				m_EnergyCostsMultipliers.Add(item.EnergyCostsMultiplier);
			}
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
			{
				Base item = GlobalItemsManager.Main.GlobalItems[i];
				Assert.AreNotEqual<float>(0, item.EnergyCostsMultiplier);
				if (item.EnergyCostsMultiplier == 1)
					continue;
				m_EnergyCostsMultipliers.Add(item.EnergyCostsMultiplier);
			}
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			m_EnergyCostsMultipliers = new List<float>();
			
			OnEnergyChanged = new Action<Energy, float>(DefaultOnEnergyChanged);
			OnFinishedSpawnSetup += delegate {
				RecalculateEnergyMax();
				RecalculateEnergyRechargeEachSecond();
				RecalculateEnergyCostsMultipliers();

				EnergyCurrent = EnergyMax;
			};
		}

		#endregion Setup
		#region Anccessors

		[Inspect, Group("Runtime"), ReadOnly]
		public float EnergyPercentile
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return (m_EnergyCurrent / m_EnergyMax) * 100;
			}
		}

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;

			return newItem.CanBeAddedToEnergy ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
		}

		#endregion Anccessors
		#region Events

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			if (null == GetComponent<DynamicAgent>())
				return;
			if (null == GetComponent<DynamicAgent>().AgentDefinition)
				return;

			// Regenerate Energy Tick
			if (false == GetComponent<Health>().IsDestroyed)
			{
				float curEnergyMax = EnergyMax;
				float curEnergyRecharge = EnergyRechargeEachSecond;

				EnergyCurrent = Math.Min(curEnergyMax, m_EnergyCurrent + (curEnergyRecharge * Time.fixedDeltaTime));
			}
		}

		#endregion Events
		#region Main

		public bool LowerEnergy(float amount)
		{
			float mod = amount;

			for (int i = 0; i < m_EnergyCostsMultipliers.Count; i++)
			{
				mod *= m_EnergyCostsMultipliers[i];
			}

			//Debug.Log("LowerEnergy"+amount);

			if (m_EnergyCurrent > mod)
			{
				m_EnergyCurrent -= mod;
				return true;
			}
			return false;
		}

		#endregion Main
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();
			public float energyCurrent;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);
			serialized.energyCurrent = m_EnergyCurrent;

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			for (int i = 0; i < serialized.items.Count; i++)
				AddItem(Base.GetFromUniqueName(serialized.items[i]));
			EnergyCurrent = serialized.energyCurrent;
		}

		#endregion Serialization
	}
}
