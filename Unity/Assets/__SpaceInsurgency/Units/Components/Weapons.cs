using SharedCode;
using System;
using System.Collections.Generic;
using AdvancedInspector;
using UnityEngine;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Weapons : EquippableComponent
	{
		#region Setup

		#endregion Setup
		#region Main

		// http://stackoverflow.com/questions/581555/enum-as-flag-using-setting-and-shifting
		[FlagsAttribute]
		public enum AttackType
		{
			None = 0,
			Strafing = (1 << 0),
			Orbit = (1 << 1),
			Defense = (1 << 2),
		};

		public bool CanDoAttackType(AttackType at)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Base item = Items[i].ItemDefinition;
				if (item.AttackType.Has<AttackType>(at))
					return true;
			}

			return false;
		}
		public void GetShortestRanges(out float shortestLongRange, out float shortestStrafing)
		{
			shortestLongRange = float.MaxValue;
			shortestStrafing = float.MaxValue;

			for (int i = 0; i < Items.Count; i++)
			{
				Base item = Items[i].ItemDefinition;

				if (item.AttackType.Has<AttackType>(AttackType.Orbit))
				{
					float maxRange = item.FireMaxRange;
					if (maxRange < shortestLongRange)
						shortestLongRange = maxRange;
				}
				else if (item.AttackType.Has<AttackType>(AttackType.Strafing))
				{
					float maxRange = item.FireMaxRange;
					if (maxRange < shortestStrafing)
						shortestStrafing = maxRange;
				}
			}
		}

		#endregion Main
		#region Anccessors

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;

			return newItem.CanBeAddedToWeapons ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
		}

		[Inspect, Group("Runtime")]
		public float FireMaxRange
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				float range = float.MinValue;
				for (int i = 0; i < Items.Count; i++)
					if (Items[i].ItemDefinition.FireMaxRange > range)
						range = Items[i].ItemDefinition.FireMaxRange;
				return range;
			}
		}

		[Inspect, Group("Runtime")]
		public float FireMinRange
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				float range = float.MaxValue;
				for (int i = 0; i < Items.Count; i++)
					if (Items[i].ItemDefinition.FireMinRange < range)
						range = Items[i].ItemDefinition.FireMinRange;
				return range;
			}
		}

		[Inspect, Group("Runtime")]
		public float OrbitMaxRange
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				float range = float.MinValue;
				for (int i = 0; i < Items.Count; i++)
					if (Items[i].ItemDefinition.AttackType == AttackType.Orbit)
						if (Items[i].ItemDefinition.OrbitMaxRange > range)
							range = Items[i].ItemDefinition.OrbitMaxRange;
				return range;
			}
		}

		[Inspect, Group("Runtime")]
		public float OrbitMinRange
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				float range = float.MaxValue;
				for (int i = 0; i < Items.Count; i++)
					if (Items[i].ItemDefinition.AttackType == AttackType.Orbit)
						if (Items[i].ItemDefinition.OrbitMinRange < range)
							range = Items[i].ItemDefinition.OrbitMinRange;
				return range;
			}
		}

		[Inspect, ReadOnly, Group("Runtime")]
		public override int ItemsMax
		{
			get
			{
				return GetComponent<DynamicAgent>().AgentDefinition.WeaponSlots;
			}
			set
			{
				
			}
		}


		#endregion Anccessors
		#region Events

		public void CheckWeaponsFire(Weapons.AttackType attackType)
		{
			if (null == GetComponent<Sensors>())
				return;
			if (null == GetComponent<Sensors>().Target)
				return;

			// Inform all items that it may be time to attack.
			float targetSqrDistance = transform.position.SqrDistance(GetComponent<Sensors>().Target.transform.position);

			List<Entry> items = GetComponent<Weapons>().Items;
			for (int i = 0; i < items.Count; i++)
			{
				Weapons.AttackType itemAttackType = items[i].ItemDefinition.AttackType;
				if (itemAttackType.Has<Weapons.AttackType>(attackType) || itemAttackType.Has<Weapons.AttackType>(AttackType.Defense))
				{
					Entry entry = items[i];
					entry.ItemDefinition.ActivateIfAppropriate(this, entry, GetComponent<Sensors>().Target, targetSqrDistance);
				}
			}
		}

		#endregion Events
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);

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