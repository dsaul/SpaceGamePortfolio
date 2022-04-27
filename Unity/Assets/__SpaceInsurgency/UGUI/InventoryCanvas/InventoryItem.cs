using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using SpaceInsurgency.Tooltip;

namespace SpaceInsurgency.Inventory
{
	[AdvancedInspector]
	public class InventoryItem : SharedCode.Behaviours.Base
	{
		#region Variables

		Base m_RepresentingItem;
		[Inspect, ReadOnly, Group("Runtime")]
		public Base RepresentingItem
		{
			get { return m_RepresentingItem; }
			set 
			{ 
				m_RepresentingItem = value;

				if (null != m_RepresentingItem)
				{
					m_ItemImage.sprite = m_RepresentingItem.InventorySprite;

					//Debug.Log("m_Draggable " + m_Draggable);

					HasTooltip hasTooltip = m_Draggable.GetComponent<HasTooltip>();
					hasTooltip.TooltipText = m_RepresentingItem.TooltipText;
				}
			}
		}

		[SerializeField]
		InventoryItemDraggable m_Draggable;
		[Inspect, Group("Editor")]
		public InventoryItemDraggable Draggable
		{
			get { return m_Draggable; }
			set { m_Draggable = value; }
		}


		EquippableComponentDisplay m_SourceDisplay;
		[Inspect, Group("Runtime"), ReadOnly]
		public EquippableComponentDisplay SourceDisplay
		{
			get { return m_SourceDisplay; }
			set { m_SourceDisplay = value; }
		}


		int m_RepresentingItemCount;
		[Inspect, ReadOnly, Group("Runtime")]
		public int RepresentingItemCount
		{
			get { return m_RepresentingItemCount; }
			set { m_RepresentingItemCount = value; }
		}

		[Inspect, Group("Editor")]
		public Image m_ItemImage;

		#endregion Variables

		
		

		
	}
}
