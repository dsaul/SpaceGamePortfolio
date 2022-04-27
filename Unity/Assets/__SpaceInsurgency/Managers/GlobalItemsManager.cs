using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using SpaceInsurgency;
using AdvancedInspector;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class GlobalItemsManager : SharedCode.Behaviours.InstanceTracked<GlobalItemsManager>
	{
		#region Variables

		List<Base> m_GlobalItems;
		[Inspect, ReadOnly]
		public List<Base> GlobalItems
		{
			get { return m_GlobalItems; }
			set { m_GlobalItems = value; }
		}

		#endregion Variables
		#region Setup

		public event Action<GlobalItemsManager, Base> OnAddedItem; // GlobalItemsManager source, Item newItem
		public event Action<GlobalItemsManager, Base> OnRemovedItem; // GlobalItemsManager source, Item removedItem

		protected override void Awake()
		{
			base.Awake();

			m_GlobalItems = new List<Base>();

			OnAddedItem = new Action<GlobalItemsManager, Base>(DefaultOnAddedItem);
			OnRemovedItem = new Action<GlobalItemsManager, Base>(DefaultOnRemovedItem);
		}

		protected override void Start()
		{
			base.Start();

			UpdateCache();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SetTimer(1f, true, OnTimeTickOneSecond);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(OnTimeTickOneSecond);
		}

		#endregion Setup
		#region Adding Equipment

		void DefaultOnAddedItem(GlobalItemsManager source, Base newItem)
		{
			m_GlobalItems.Add(newItem);

			UpdateCache();
		}

		public void AddItem(Base newItem)
		{
			OnAddedItem(this, newItem);
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

		void DefaultOnRemovedItem(GlobalItemsManager source, Base removedItem)
		{
			m_GlobalItems.Remove(removedItem);

			UpdateCache();
		}

		public void RemoveItem(Base item)
		{
			OnRemovedItem(this, item);
		}

		void RemoveAllItems()
		{
			while (0 != m_GlobalItems.Count)
				RemoveItem(m_GlobalItems[0]);
		}

		#endregion Removing Equipment
		#region Income

		float m_CachedItemsCreditsIncomePerSecond = 0;

		#endregion Income
		#region Store Price Multiplier

		float m_CachedItemsStorePriceMultiplier = 1;
		public float ItemsStorePriceMultiplier
		{
			get
			{
				return m_CachedItemsStorePriceMultiplier;
			}
		}

		#endregion Store Price Multiplier
		#region Main

		public void UpdateCache()
		{
			float income = 0;
			float priceMultiplier = 1;

			for (int i = 0; i < m_GlobalItems.Count; i++)
			{
				Base item = m_GlobalItems[i];
				income += item.CreditsPerSecondModifier;
				
				Assert.AreNotEqual<float>(0, item.StorePriceMultiplier);
				priceMultiplier *= item.StorePriceMultiplier;
			}

			m_CachedItemsCreditsIncomePerSecond = income;
			m_CachedItemsStorePriceMultiplier = priceMultiplier;
		}

		void OnTimeTickOneSecond()
		{
			SaveStateManager.First.PlayerCredits += m_CachedItemsCreditsIncomePerSecond;
		}

		#endregion Main
	}
}