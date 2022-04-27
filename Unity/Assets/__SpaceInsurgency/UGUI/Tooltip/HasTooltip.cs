using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SharedCode;
using AdvancedInspector;
using System.Collections;

namespace SpaceInsurgency.Tooltip
{
	public class HasTooltip : SharedCode.Behaviours.Base, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{

		[SerializeField]
		string m_TooltipText;
		[Inspect, Group("Editor")]
		public string TooltipText
		{
			get { return m_TooltipText; }
			set { m_TooltipText = value; }
		}

		bool m_IsPointerOver = false;

		[Inspect, ReadOnly]
		public bool IsPointerOver
		{
			get { return m_IsPointerOver; }
			private set { m_IsPointerOver = value; }
		}

		bool m_IsDragActive = false;
		[Inspect, ReadOnly]
		public bool IsDragActive
		{
			get { return m_IsDragActive; }
			set { m_IsDragActive = value; }
		}

		


		public void OnPointerEnter(PointerEventData eventData)
		{
			//Tooltip.Main.Text = m_TooltipText;
			m_IsPointerOver = true;
			UpdateVisible();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			//Tooltip.Main.Text = null;
			m_IsPointerOver = false;
			UpdateVisible();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			m_IsDragActive = true;
			//Tooltip.Main.Text = null;
			UpdateVisible();
			//Debug.Log("OnBeginDrag");
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			m_IsDragActive = true;
			//Tooltip.Main.Text = null;
			UpdateVisible();
		}

		public void OnDrag(PointerEventData eventData)
		{
			//Tooltip.Main.Text = null;
			UpdateVisible();
		}

		void UpdateVisible()
		{
			if (false == IsDragActive && true == IsPointerOver)
				Tooltip.Main.Text = m_TooltipText;
			else
				Tooltip.Main.Text = null;
		}
	}
}



