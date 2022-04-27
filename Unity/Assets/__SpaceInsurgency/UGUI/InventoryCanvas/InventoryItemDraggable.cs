using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using SpaceInsurgency.Items;

namespace SpaceInsurgency.Inventory
{
	[AdvancedInspector]
	public class InventoryItemDraggable : SharedCode.Behaviours.Base, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public static InventoryItem itemBeingDragged;

		Transform m_DragBeginParent;
		Vector3 m_DragBeginItemPosition;
		Vector3 m_DragBeginMousePosition;
		Vector3 m_DragBeginMouseOffset;

		[SerializeField]
		InventoryItem m_InventoryItem;
		[Inspect, Group("Editor")]
		public InventoryItem InventoryItem
		{
			get { return m_InventoryItem; }
			set { m_InventoryItem = value; }
		}


		
		public void OnBeginDrag(PointerEventData eventData)
		{
			itemBeingDragged = m_InventoryItem;
			m_DragBeginItemPosition = transform.position;
			m_DragBeginMousePosition = Input.mousePosition;
			m_DragBeginMouseOffset = transform.InverseTransformPoint(m_DragBeginMousePosition);

			m_DragBeginParent = transform.parent;
			transform.parent = InventoryCanvas.Main.DragRootTransform;
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			transform.position = Input.mousePosition - m_DragBeginMouseOffset;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			itemBeingDragged = null;
			GetComponent<CanvasGroup>().blocksRaycasts = true;
			transform.parent = m_DragBeginParent;
			transform.position = m_DragBeginItemPosition;
		}
	}
}