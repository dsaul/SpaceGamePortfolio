using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using SpaceInsurgency.Items;

namespace SpaceInsurgency.Inventory
{
	[AdvancedInspector]
	public class EquippableComponentDisplay : SharedCode.Behaviours.Base, IDropHandler, IPointerEnterHandler, IPointerExitHandler
	{
		#region Variables

		EquippableComponent m_EquippableComponent;
		[Inspect, ReadOnly, Group("Runtime")]
		public EquippableComponent EquippableComponent
		{
			get { return m_EquippableComponent; }
			set
			{
				if (null != m_EquippableComponent)
				{
					m_EquippableComponent.OnAddedItem -= EquippableComponent_OnAddedItem;
					m_EquippableComponent.OnRemovedItem -= EquippableComponent_OnRemovedItem;
				}
				
				m_EquippableComponent = value;

				if (null != m_EquippableComponent)
				{
					m_EquippableComponent.OnAddedItem += EquippableComponent_OnAddedItem;
					m_EquippableComponent.OnRemovedItem += EquippableComponent_OnRemovedItem;
				}

				Relay();
			}
		}

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

		[Inspect, Group("Editor")]
		public GameObject m_DropZoneIndicator;
		

		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();

			Relay();

			m_DropZoneIndicator.SetActive(false);
		}

		#endregion Setup
		#region Events

		void EquippableComponent_OnAddedItem(EquippableComponent source, Base newItem)
		{
			Relay();
		}

		void EquippableComponent_OnRemovedItem(EquippableComponent source, EquippableComponent.Entry newItem)
		{
			Relay();
		}

		public void OnDrop(PointerEventData eventData)
		{
			do
			{
				InventoryItem itemBeingDragged = InventoryItemDraggable.itemBeingDragged;
				if (null == itemBeingDragged)
					break;

				// Must have come from somewhere.
				EquippableComponentDisplay sourceEquippableComponentDisplay = itemBeingDragged.SourceDisplay;
				if (this == sourceEquippableComponentDisplay)
					break;

				// The item and how many we're moving.
				Base item = itemBeingDragged.RepresentingItem;
				int itemCount = itemBeingDragged.RepresentingItemCount;

				EquippableComponent destinationEquippableComponent = EquippableComponent;
				if (null == destinationEquippableComponent)
					break;

				EquippableComponent sourceEquippableComponent = sourceEquippableComponentDisplay.EquippableComponent;
				if (null == sourceEquippableComponent)
					break;

				EquippableComponent.CanAddItemAnswer answer = destinationEquippableComponent.CanAddItem(item, itemCount);
				if (SpaceInsurgency.EquippableComponent.CanAddItemAnswer.Yes == answer)
				{
					sourceEquippableComponent.RemoveItem(item, itemCount);
					destinationEquippableComponent.AddItem(item, itemCount);
				}

				Debug.Log("OnDrop " + itemBeingDragged);

			} while (false);


			m_DropZoneIndicator.SetActive(false);
			
			
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			InventoryItem itemBeingDragged = InventoryItemDraggable.itemBeingDragged;
			if (null == itemBeingDragged)
				return;

			// Must have come from somewhere.
			EquippableComponentDisplay sourceEquippableComponentDisplay = itemBeingDragged.SourceDisplay;
			if (this == sourceEquippableComponentDisplay)
				return;

			// The item and how many we're moving.
			Base item = itemBeingDragged.RepresentingItem;
			int itemCount = itemBeingDragged.RepresentingItemCount;

			EquippableComponent destinationEquippableComponent = EquippableComponent;
			if (null == destinationEquippableComponent)
				return;

			EquippableComponent sourceEquippableComponent = sourceEquippableComponentDisplay.EquippableComponent;
			if (null == sourceEquippableComponent)
				return;

			EquippableComponent.CanAddItemAnswer answer = destinationEquippableComponent.CanAddItem(item, itemCount);
			if (SpaceInsurgency.EquippableComponent.CanAddItemAnswer.Yes == answer)
			{
				m_DropZoneIndicator.SetActive(true);
			}

			//Debug.Log("OnPointerEnter");
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			m_DropZoneIndicator.SetActive(false);

			//Debug.Log("OnPointerExit");
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
				GameObject.Destroy(children[i].gameObject);

			if (null == m_EquippableComponent)
				return;

			using (IEnumerator<KeyValuePair<Base,int>> e = m_EquippableComponent.ItemsCountedSet.UniqueEnumerable.GetEnumerator())
				while (e.MoveNext())
				{
					Base item = e.Current.Key;
					int count = e.Current.Value;

					GameObject itemObj = GameObject.Instantiate<GameObject>(m_ItemPrefab);
					InventoryItem inventoryItem = itemObj.GetComponent<InventoryItem>();
					inventoryItem.RepresentingItem = item;
					inventoryItem.RepresentingItemCount = count;
					inventoryItem.SourceDisplay = this;
					itemObj.transform.SetParent(m_ItemParentTransform.transform, false);
				}


		}

		#endregion Main




		
	}
}