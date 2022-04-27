using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Vectrosity;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Shield : EquippableComponent
	{
		#region Variables

		float m_ShieldCurrent = 100f;

		[Inspect, Group("Runtime")]
		public float ShieldCurrent
		{
			get
			{
				if (null != GetComponent<Health>() && true == GetComponent<Health>().IsDestroyed)
					return 0f;
				else
					return m_ShieldCurrent.Clamp<float>(0, ShieldMax);
			}
			set
			{
				if (false == Application.isPlaying)
					return;

				OnShieldChanged(this, value);
			}
		}

		float m_ShieldMax;

		[Inspect, Group("Runtime")]
		public float ShieldMax
		{
			get { return m_ShieldMax; }
		}

		float m_ShieldRechargeEachSecond;

		[Inspect, Group("Runtime")]
		public float ShieldRechargeEachSecond
		{
			get { return m_ShieldRechargeEachSecond; }
		}

		#endregion Variables
		#region Event Definitions

		public event Action<Shield, float> OnShieldChanged; // Shield source, float newShield

		#endregion Event Definitions
		#region Setup

		protected override void Awake()
		{
			base.Awake();
			OnShieldChanged = new Action<Shield, float>(DefaultOnShieldChanged);
			OnFinishedSpawnSetup += delegate {
				RecalculateShieldMax();
				RecalculateShieldRechargeEachSecond();

				ShieldCurrent = ShieldMax;
			};
		}

		#endregion Setup
		#region Main

		void RecalculateShieldMax()
		{
			float r = GetComponent<DynamicAgent>().AgentDefinition.ShieldMaxBase;

			for (int i = 0; i < Items.Count; i++)
				r += Items[i].ItemDefinition.MaxShieldModifier;
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
				r += GlobalItemsManager.Main.GlobalItems[i].MaxShieldModifier;

			m_ShieldMax = r;
		}

		void RecalculateShieldRechargeEachSecond()
		{
			float r = GetComponent<DynamicAgent>().AgentDefinition.ShieldRechargeBase;

			for (int i = 0; i < Items.Count; i++)
				r += Items[i].ItemDefinition.ShieldRechargeModifier;
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
				r += GlobalItemsManager.Main.GlobalItems[i].ShieldRechargeModifier;

			m_ShieldRechargeEachSecond = r;
		}

		void UpdateShader()
		{
			if (null == GetComponent<DynamicAgent>())
				return;

			Transform shipShieldObject = GetComponent<DynamicAgent>().ShipShieldObject;
			if (null == shipShieldObject)
				return;

			float ratio = m_ShieldCurrent / m_ShieldMax;

			Renderer[] renderers = shipShieldObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < renderers.Length; i++)
			{
				Material mat = renderers[i].material;
				if (mat.HasProperty("_Luminance"))
				{
					mat.SetFloat("_Luminance", ratio);
				}
				else if (mat.HasProperty("_Intensity"))
				{
					mat.SetFloat("_Intensity", ratio);
				}
			}


		}

		public void TakeDamage(float damage, SpaceObject.DamageType damageType, out float remainder, out float actualDamageTaken)
		{
			Assert.IsFalse(damage < 0);

			remainder = 0f;

			// We need to save the start shield for the end to find out the actual damage taken.
			float startShield = ShieldCurrent;

			// Some types of damage are extra effective vs shields. We need to take this into account.
			float modifiedDamage = damage;
			if (SpaceObject.DamageType.EMP == damageType)
			{
				modifiedDamage *= 7;
			}

			// Apply the damage, get the remainder if it exists.
			float newShield = startShield - modifiedDamage;
			if (newShield < 0)
			{
				remainder = Math.Abs(newShield);
				newShield = 0f;
			}

			ShieldCurrent = newShield;

			// We reduce the remainder by the damage multiplication before so that 
			// it more accurately represents what should be passed on to the armour.
			if (SpaceObject.DamageType.EMP == damageType)
			{
				remainder /= 7;
			}

			actualDamageTaken = startShield - ShieldCurrent;
		}

		#endregion Main
		#region Anccessors

		[Inspect, Group("Runtime")]
		public float ShieldPercentile
		{
			get { return (m_ShieldCurrent / m_ShieldMax) * 100; }
		}

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;

			return newItem.CanBeAddedToShield ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
		}

		#endregion Anccessors
		#region Events

		void DefaultOnShieldChanged(Shield source, float newShield)
		{
			m_ShieldCurrent = newShield.Clamp<float>(0, ShieldMax);
			UpdateShader();
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			if (null == GetComponent<DynamicAgent>())
				return;
			if (null == GetComponent<DynamicAgent>().AgentDefinition)
				return;

			// RegenerateShieldTick
			if (false == GetComponent<Health>().IsDestroyed)
			{
				float curShieldMax = m_ShieldMax;
				float curShieldRecharge = ShieldRechargeEachSecond;
				if (m_ShieldCurrent < curShieldMax && GetComponent<Energy>().EnergyPercentile > 40)
				{
					float e = curShieldRecharge * Time.fixedDeltaTime * 4;

					if (GetComponent<Energy>().LowerEnergy(e))
					{
						ShieldCurrent = Math.Min(curShieldMax, m_ShieldCurrent + (curShieldRecharge * Time.fixedDeltaTime));
						UpdateShader();
					}
				}
			}

		}

		#endregion Events
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();
			public float shieldCurrent;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);
			serialized.shieldCurrent = m_ShieldCurrent;

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



















































