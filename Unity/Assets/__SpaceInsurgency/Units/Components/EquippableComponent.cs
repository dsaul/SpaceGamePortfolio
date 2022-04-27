using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using AdvancedInspector;
using SharedCode;
using SharedCode.Types;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class EquippableComponent : SharedCode.Behaviours.Base
	{
		#region Types

		[AdvancedInspector]
		public class Entry
		{
			Base m_ItemDefinition;

			[Inspect, ReadOnly]
			public Base ItemDefinition
			{
				get { return m_ItemDefinition; }
				set { m_ItemDefinition = value; }
			}

			float m_Cooldown;

			[Inspect, ReadOnly]
			public float LastCooldown
			{
				get { return m_Cooldown; }
				set { m_Cooldown = value; }
			}

			bool m_ToggledOn = true;

			[Inspect, ReadOnly]
			public bool ToggledOn
			{
				get { return m_ToggledOn; }
				set { m_ToggledOn = value; }
			}

			GameObject m_GameObject;
			[Inspect, ReadOnly]
			public GameObject GameObject
			{
				get { return m_GameObject; }
				set { m_GameObject = value; }
			}

		}

		#endregion
		#region Event Definitions

		Action<EquippableComponent, Base> _OnAddedItem; // EquippableComponent source, Base newItem
		public Action<EquippableComponent, Base> OnAddedItem
		{
			get
			{
				if (null == _OnAddedItem)
					_OnAddedItem = new Action<EquippableComponent, Base>(delegate { });
				return _OnAddedItem;
			}
			set { _OnAddedItem = value; }
		}

		Action<EquippableComponent, Entry> _OnRemovedItem; // EquippableComponent source, Entry removedItem
		public Action<EquippableComponent, Entry> OnRemovedItem
		{
			get
			{
				if (null == _OnAddedItem)
					_OnRemovedItem = new Action<EquippableComponent, Entry>(delegate { });
				return _OnRemovedItem;
			}
			set { _OnRemovedItem = value; }
		}

		#endregion Event Definitions
		#region Variables

		
		List<Entry> m_Items;

		[Inspect, ReadOnly, Group("Runtime")]
		public List<Entry> Items
		{
			get
			{
				if (null == m_Items)
					m_Items = new List<Entry>();
				return m_Items;
			}
		}

		CountedSet<Base> m_ItemsCountedSet;
		[Inspect, ReadOnly, Group("Runtime")]
		public CountedSet<Base> ItemsCountedSet
		{
			get
			{
				if (null == m_ItemsCountedSet)
					m_ItemsCountedSet = new CountedSet<Base>();
				return m_ItemsCountedSet;
			}
		}

		public void SetItems(IEnumerable<Base> items)
		{
			RemoveAllItems();

			using (IEnumerator<Base> enumerator = items.GetEnumerator())
				while (enumerator.MoveNext())
					AddItem(enumerator.Current);
		}

		public int m_ItemsMax = 6;

		[Inspect, ReadOnly, Group("Runtime")]
		public virtual int ItemsMax
		{
			get { return m_ItemsMax; }
			set { m_ItemsMax = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();
		}

		#endregion Setup
		#region Anccessors



		#endregion Anccessors
		#region Equipment





		public virtual IEnumerable<Base> ItemsShownInActionArea
		{
			get
			{
				for (int i = 0; i < Items.Count; i++)
				{
					Base item = Items[i].ItemDefinition;
					if (false == item.CanActivate)
						continue;
					yield return item;
				}
			}
		}

		[Inspect, Group("Runtime")]
		public float ItemsTotalWeight
		{
			get
			{
				float r = 0;
				using (IEnumerator<KeyValuePair<Base, int>> e = ItemsCountedSet.UniqueEnumerable.GetEnumerator())
					while (e.MoveNext())
						r += (e.Current.Key.Weight * e.Current.Value);
				return r;
			}
		}

		public int OccurancesOfItem(Base item)
		{
			return ItemsCountedSet.CountFor(item);
		}

		public enum CanAddItemAnswer
		{
			Yes = 0,
			TooMuchWeight = 1,
			NoSlotAvailable = 2,
			ItemDefinitionDisallows = 3
		};

		public virtual CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			Assert.IsNotNull<Base>(newItem);
			
			float totalWeight = 0f;
			if (null != GetComponent<Sensors>())
				totalWeight += GetComponent<Sensors>().ItemsTotalWeight;
			if (null != GetComponent<Engines>())
				totalWeight += GetComponent<Engines>().ItemsTotalWeight;
			if (null != GetComponent<Energy>())
				totalWeight += GetComponent<Energy>().ItemsTotalWeight;
			if (null != GetComponent<Health>())
				totalWeight += GetComponent<Health>().ItemsTotalWeight;
			if (null != GetComponent<Shield>())
				totalWeight += GetComponent<Shield>().ItemsTotalWeight;
			if (null != GetComponent<Weapons>())
				totalWeight += GetComponent<Weapons>().ItemsTotalWeight;
			if (null != GetComponent<Cargo>())
				totalWeight += GetComponent<Cargo>().ItemsTotalWeight;

			if (((newItem.Weight * count) + totalWeight) > GetComponent<DynamicAgent>().AgentDefinition.WeightMaxBase)
				return Cargo.CanAddItemAnswer.TooMuchWeight;

			if (Items.Count + count > m_ItemsMax)
				return Cargo.CanAddItemAnswer.NoSlotAvailable;

			return Cargo.CanAddItemAnswer.Yes;
		}
		#endregion Equipment
		#region Adding Equipment

		public void AddItem(Base NewItem, int Count, Action<Base, int, CanAddItemAnswer> AddAnswerCallback = null)
		{
			for (int i = 0; i < Count; i++)
			{
				CanAddItemAnswer answer = AddItem(NewItem);
				if (null != AddAnswerCallback)
					AddAnswerCallback(NewItem, Count, answer);
			}
		}

		public CanAddItemAnswer AddItem(Base newItem)
		{
			CanAddItemAnswer answer = CanAddItem(newItem, 1);
			if (Cargo.CanAddItemAnswer.Yes != answer)
			{
				Debug.LogWarning("CanAddItemAnswer.Yes != CanAddItem(newItem) newItem=" + newItem + " answer= " + answer);
				return answer;
			}

			newItem.OnAddedTo(this);

			Items.Add(new Entry() { ItemDefinition = newItem });
			ItemsCountedSet.Add(newItem);
			
			OnAddedItem(this, newItem);

			return answer;
		}

		public void AddItems(IEnumerable<Base> newItems)
		{
			using (IEnumerator<Base> enumerator = newItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Base newItem = enumerator.Current;
					AddItem(newItem);
				}
			}
		}

		#endregion Adding Equipment
		#region Removing Equipment

		public void RemoveItem(Entry item)
		{
			//Debug.Log("#### " + item + " " + OnRemovedItem);

			item.ItemDefinition.OnRemovedFrom(this);



			Items.Remove(item);
			ItemsCountedSet.Remove(item.ItemDefinition);

			OnRemovedItem(this, item);
		}

		public void RemoveItem(Base itemDef)
		{
			//Debug.Log("RemoveItem " + itemDef);
			
			Entry entry = null;
			for (int i = 0; i < Items.Count; i++)
			{
				entry = Items[i];
				if (entry.ItemDefinition == itemDef)
					break;
			}

			//Debug.Log("### " + entry);

			if (null != entry)
				RemoveItem(entry);
		}

		public void RemoveItem(Base itemDef, int count)
		{
			for (int i=0; i<count; i++)
				RemoveItem(itemDef);
		}

		public void RemoveAllItems()
		{
			while (0 != Items.Count)
				RemoveItem(Items[0]);
		}

		#endregion Removing Equipment
	}
}
