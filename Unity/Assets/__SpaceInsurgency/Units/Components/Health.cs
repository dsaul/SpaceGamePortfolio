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
	public class Health : EquippableComponent
	{
		#region Event Definitions

		// Damage
		public event Action<Health, float> OnDamageTaken; // Health source, float actualDamageTaken

		// Blown Up
		public event Action<Health> OnShipDestroyed; // Health source
		public event Action<Health, float> OnHealthChanged; // Health source, float newHealth

		#endregion Event Definitions
		#region Variables

		float m_HealthCurrent;
		[Inspect, Group("Runtime")]
		public float HealthCurrent
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				if (true == IsDestroyed)
					return 0f;

				return m_HealthCurrent.Clamp<float>(0, m_HealthMax);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				if (null != OnHealthChanged)
					OnHealthChanged(this, value);
			}
		}
		void DefaultOnHealthChanged(Health source, float newHealth)
		{
			m_HealthCurrent = newHealth.Clamp<float>(0, HealthMax);
		}

		float m_HealthMax;
		[Inspect, Group("Runtime"), ReadOnly]
		public float HealthMax
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return m_HealthMax;
			}
		}
		void RecalculateHealthMax()
		{
			if (false == Application.isPlaying)
			{
				m_HealthMax = float.NaN;
				return;
			}

			float r = GetComponent<DynamicAgent>().AgentDefinition.HealthMaxBase;

			for (int i = 0; i < Items.Count; i++)
				r += Items[i].ItemDefinition.MaxHealthModifier;
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
				r += GlobalItemsManager.Main.GlobalItems[i].MaxHealthModifier;

			m_HealthMax = r;
		}

		float m_HealthRechargeEachSecond;
		[Inspect, Group("Runtime"), ReadOnly]
		public float HealthRechargeEachSecond
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return m_HealthRechargeEachSecond;
			}
		}
		void RecalculateHealthRechargeEachSecond()
		{
			if (false == Application.isPlaying)
			{
				m_HealthRechargeEachSecond = float.NaN;
				return;
			}

			float r = GetComponent<DynamicAgent>().AgentDefinition.HealthRechargeBase;

			for (int i = 0; i < Items.Count; i++)
				r += Items[i].ItemDefinition.HealthRechargeModifier;
			for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
				r += GlobalItemsManager.Main.GlobalItems[i].HealthRechargeModifier;

			m_HealthRechargeEachSecond = r;
		}

		bool m_IsDestroyed = false;
		[Inspect, Group("Runtime"), ReadOnly]
		public bool IsDestroyed
		{
			get
			{
				return m_IsDestroyed;
			}
		}

		#endregion Variables
		#region Setup

		

		protected override void Awake()
		{
			base.Awake();

			OnDamageTaken = new Action<Health, float>(delegate { });
			OnShipDestroyed = new Action<Health>(DefaultOnShipDestroyed);
			OnHealthChanged = new Action<Health, float>(DefaultOnHealthChanged);
			OnFinishedSpawnSetup += delegate {
				RecalculateHealthMax();
				RecalculateHealthRechargeEachSecond();
				HealthCurrent = HealthMax;
			};
		}

		#endregion Setup
		#region Anccessors

		[Inspect, Group("Runtime")]
		public float HealthPercentile
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return (m_HealthCurrent / m_HealthMax) * 100;
			}
		}

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;

			return newItem.CanBeAddedToHealth ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
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

			// if this ship is damaged enoough just blow it 
			// up and don't bother with anything else.
			if (HealthCurrent <= 0)
			{
				OnShipDestroyed(this);
				return;
			}

			//Regenerate Health Tick
			if (false == GetComponent<Health>().IsDestroyed)
			{
				float curHealthMax = m_HealthMax;
				float curHealthRecharge = HealthRechargeEachSecond;

				if (HealthCurrent < curHealthMax)
				{
					HealthCurrent = Math.Min(curHealthMax, HealthCurrent + (curHealthRecharge * Time.fixedDeltaTime));
				}
			}
		}

		#endregion Events
		#region Main

		public void TakeDamage(float damage, SpaceObject.DamageType damageType, out float remainder, out float actualDamageTaken)
		{
			Assert.IsFalse(damage < 0);

			remainder = 0f;

			// We need to save the start armour for the end to find out the actual damage taken.
			float startHelath = HealthCurrent;

			// Some types of damage are extra effective vs shields. We need to take this into account.
			float modifiedDamage = damage;
			if (SpaceObject.DamageType.EMP == damageType)
			{
				modifiedDamage /= 50; // Really I don't think EMP's should be doing any damage, but what if there is nothing left?
			}

			// Apply the damage, get the remainder if it exists.
			float newHealth = startHelath - modifiedDamage;
			if (newHealth < 0)
			{
				remainder = Math.Abs(newHealth);
				newHealth = 0f;
			}

			HealthCurrent = newHealth;

			// Undo the modifications to the damage for the remainder.
			if (SpaceObject.DamageType.EMP == damageType)
			{
				remainder *= 50;
			}

			actualDamageTaken = startHelath - HealthCurrent;
			OnDamageTaken(this, actualDamageTaken);
		}

		float blowUpEffectTime;

		void DefaultOnShipDestroyed(Health source)
		{
			if (true == m_IsDestroyed)
				return;

			ShipModelParts parts = GetComponent<DynamicAgent>().Parts;

			// If there is a blowUpDamageAmount, damage everything within explosion area.
			if (true == parts.HasDestructionAoEDamage)
			{
				Vector3 thisPos = transform.position;

				List<DynamicAgent> ships = DynamicAgent.Instances;
				for (int i = 0; i < ships.Count; i++)
				{
					DynamicAgent ship = ships[i];
					if (thisPos.SqrDistance(ship.transform.position) < parts.DestructionAoEDamageRange * parts.DestructionAoEDamageRange)
						ship.GetComponent<SpaceObject>().TakeDamage(parts.DestructionAoEDamageAmount, SpaceObject.DamageType.Regular);
				}
			}

			m_IsDestroyed = true;

			SelectionManager.Main.DeSelect(GetComponent<SpaceObject>());


			GameObject.Instantiate(GetComponent<DynamicAgent>().AgentDefinition.ExplosionEffectPrefab, transform.position, Quaternion.identity);
			blowUpEffectTime = Time.time;

			AudioManager.Main.PlayClipAtPosition(GetComponent<DynamicAgent>().AgentDefinition.AudioOnExplode, AudioManager.Main.mixerDestruction, transform.position);

			/*MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < meshRenderers.Length; i++)
				meshRenderers[i].enabled = false;

			SGT_Thruster[] thrusters = GetComponentsInChildren<SGT_Thruster>();
			for (int i = 0; i < thrusters.Length; i++)
				thrusters[i].enabled = false;*/

			SetTimer(0.25f, true, DelayedShipDestruction);

		}

		void DelayedShipDestruction()
		{
			if (Time.time < (blowUpEffectTime + 0.75f))
				return;
			ClearTimer(DelayedShipDestruction);

			if (null != GetComponent<DynamicAgent>())
				GetComponent<DynamicAgent>().CleanRemove();
		}

		#endregion Main
		#region Utility

		[Inspect, Group("Utility")]
		public void DestroyShip()
		{
			HealthCurrent = 0f;
		}

		#endregion Utility
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();
			public float healthCurrent;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);
			serialized.healthCurrent = m_HealthCurrent;

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			for (int i = 0; i < serialized.items.Count; i++)
				AddItem(Base.GetFromUniqueName(serialized.items[i]));
			HealthCurrent = serialized.healthCurrent;
		}

		#endregion Serialization
	}
}
