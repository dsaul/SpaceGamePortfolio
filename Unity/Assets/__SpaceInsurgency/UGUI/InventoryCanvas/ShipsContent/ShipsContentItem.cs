using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency.Inventory
{
	[AdvancedInspector]
	public class ShipsContentItem : SharedCode.Behaviours.Base, IPointerClickHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
	{
		#region Variables


		ShipsContent m_ShipsContent;
		[Inspect, ReadOnly, Group("Runtime")]
		public ShipsContent ShipsContent
		{
			get { return m_ShipsContent; }
			set {
				m_ShipsContent = value;
			}
		}

		DynamicAgent m_DynamicAgent;

		[Inspect, ReadOnly, Group("Runtime")]
		public DynamicAgent DynamicAgent
		{
			get { return m_DynamicAgent; }
			set
			{
				if (null != m_DynamicAgent)
					m_DynamicAgent.OnGivenNameChanged -= DynamicAgent_OnGivenNameChanged;
				
				m_DynamicAgent = value;

				if (null != m_DynamicAgent)
					m_DynamicAgent.OnGivenNameChanged += DynamicAgent_OnGivenNameChanged;

				DynamicAgent_OnGivenNameChanged(m_DynamicAgent, null == m_DynamicAgent ? "" : m_DynamicAgent.GivenName);

				m_ShipDefinitionText.text = null == m_DynamicAgent ? "" : m_DynamicAgent.AgentDefinition.DisplayName;

				Relay();
			}
		}

		[SerializeField]
		Text m_ShipNameText;

		[Inspect, Group("Editor")]
		public Text ShipNameText
		{
			get { return m_ShipNameText; }
			set { m_ShipNameText = value; }
		}

		[SerializeField]
		Text m_ShipDefinitionText;
		[Inspect, Group("Editor")]
		public Text ShipDefinitionText
		{
			get { return m_ShipDefinitionText; }
			set { m_ShipDefinitionText = value; }
		}

		bool m_IsSelected = false;
		[Inspect, Group("Runtime"), ReadOnly]
		public bool IsSelected
		{
			get { return m_IsSelected; }
			set 
			{ 
				m_IsSelected = value;

				GetComponent<Image>().color = m_IsSelected ? new Color(0.4705882352941176f, 0.7725490196078431f, 0.8392156862745098f) : new Color(0.24f, 0.24f, 0.24f);
			}
		}

		[Inspect, Group("Editor")]
		public GameObject m_DropZoneIndicator;

		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();

			m_DropZoneIndicator.SetActive(false);
		}

		#endregion Setup
		#region Events

		protected override void OnDestroy()
		{
			base.OnDestroy();

			DynamicAgent = null;
		}

		void DynamicAgent_OnGivenNameChanged(DynamicAgent sender, string value)
		{
			m_ShipNameText.text = value;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			ShipsContent.SelectedItem = this;
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

				EquippableComponent destinationEquippableComponent = DynamicAgent.GetComponent<Cargo>() as EquippableComponent;
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

			EquippableComponent destinationEquippableComponent = DynamicAgent.GetComponent<Cargo>() as EquippableComponent;
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
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			m_DropZoneIndicator.SetActive(false);
		}

		#endregion Events
		#region Main

		[Inspect, Group("Runtime")]
		public void Relay()
		{
			
		}

		#endregion Main





		
	}
}
