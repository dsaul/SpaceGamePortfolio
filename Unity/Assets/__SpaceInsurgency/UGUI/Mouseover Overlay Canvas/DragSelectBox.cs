using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Mouseover
{
	[AdvancedInspector]
	public class DragSelectBox : SharedCode.Behaviours.Base
	{
		#region Variables

		Vector2? m_MouseDragSelectDownPos;

		[Inspect, Group("Runtime")]
		public Vector2? MouseDragSelectDownPos
		{
			get { return m_MouseDragSelectDownPos.Value; }
			set { m_MouseDragSelectDownPos = value; }
		}

		bool m_MouseDragSelectBoxActivated = false;
		[Inspect, Group("Runtime")]
		public bool MouseDragSelectBoxActivated
		{
			get { return m_MouseDragSelectBoxActivated; }
			set { m_MouseDragSelectBoxActivated = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			//SubSignal();
		}

		protected override void Start()
		{
			base.Start();

			
			SubSignal();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubSignal();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnSubSignal();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;
			if (null == InputManager.Main)
				return;

			InputManager.Main.OnPrimaryBegin += InputManager_OnPrimaryBegin;
			InputManager.Main.OnPrimaryStay += InputManager_OnPrimaryStay;
			InputManager.Main.OnPrimaryEnd += InputManager_OnPrimaryEnd;

			GetComponent<CanvasGroup>().alpha = m_MouseDragSelectDownPos == null ? 0 : 1;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == InputManager.Main)
				return;

			InputManager.Main.OnPrimaryBegin -= InputManager_OnPrimaryBegin;
			InputManager.Main.OnPrimaryStay -= InputManager_OnPrimaryStay;
			InputManager.Main.OnPrimaryEnd -= InputManager_OnPrimaryEnd;

			signalSub = false;
		}

		#endregion Setup
		#region Events

		void InputManager_OnPrimaryBegin(InputManager source)
		{
			if (true == source.IsOverUIElement)
				return;
			// If we're in the galaxy map, drag select is disabled.
			if (true == LevelGeometry.Main.IsGalaxyMap)
				return;
			
			// Get the initial click position of the mouse. No need to convert to GUI space
			// since we are using the lower left as anchor and pivot.
			m_MouseDragSelectDownPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			// The anchor is set to the same place.
			GetComponent<RectTransform>().anchoredPosition = m_MouseDragSelectDownPos.Value;
		}

		void InputManager_OnPrimaryStay(InputManager source)
		{
			if (null == m_MouseDragSelectDownPos)
				return;

			Vector2 mouseDragSelectDragPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			// We don't want to show the selection box until we went past 10 pixels away at least once.
			if (false == m_MouseDragSelectBoxActivated && m_MouseDragSelectDownPos.Value.SqrDistance(mouseDragSelectDragPos) < (10 * 10))
				return;

			GetComponent<CanvasGroup>().alpha = 1f;
			m_MouseDragSelectBoxActivated = true;

			// How far have we moved the mouse?
			Vector2 difference = mouseDragSelectDragPos - m_MouseDragSelectDownPos.Value;

			Vector2 newPos = m_MouseDragSelectDownPos.Value;

			// The following code accounts for dragging in various directions.
			if (difference.x < 0)
			{
				newPos.x = mouseDragSelectDragPos.x;
				difference.x = -difference.x;
			}
			if (difference.y < 0)
			{
				newPos.y = mouseDragSelectDragPos.y;
				difference.y = -difference.y;
			}

			// Set the anchor, width and height every frame.
			GetComponent<RectTransform>().anchoredPosition = newPos;
			GetComponent<RectTransform>().sizeDelta = difference;

		}

		void InputManager_OnPrimaryEnd(InputManager source)
		{
			do
			{
				if (null == m_MouseDragSelectDownPos)
					break;
				if (false == m_MouseDragSelectBoxActivated)
					break;
				if (true == source.IsOverUIElement)
					break;

				Vector2 mouseDragSelectDragPos = Input.mousePosition;

				Vector2 min = new Vector2(Mathf.Min(m_MouseDragSelectDownPos.Value.x, mouseDragSelectDragPos.x), Mathf.Min(m_MouseDragSelectDownPos.Value.y, mouseDragSelectDragPos.y));
				Vector2 max = new Vector2(Mathf.Max(m_MouseDragSelectDownPos.Value.x, mouseDragSelectDragPos.x), Mathf.Max(m_MouseDragSelectDownPos.Value.y, mouseDragSelectDragPos.y));

				Camera cameraGame = CameraManager.Main.CameraGame;

				if (false == source.ModifierKeyActive)
					SelectionManager.Main.DeSelectAll();

				// Ships
				List<DynamicAgent> ships = DynamicAgent.Instances;
				for (int i = 0; i < ships.Count; i++)
				{
					DynamicAgent ship = ships[i];
					if (false == ship.IsVisibleFromCamera(cameraGame))
						continue;

					Vector3 screenPoint = cameraGame.WorldToScreenPoint(ship.transform.position);

					if (screenPoint.x > min.x && screenPoint.x < max.x && screenPoint.y > min.y && screenPoint.y < max.y)
					{
						SelectionManager.Main.Select(ship.GetComponent<SpaceObject>());
					}
				}

				// Planets
				List<StaticAgent> planets = StaticAgent.Instances;
				for (int i = 0; i < planets.Count; i++)
				{
					StaticAgent planet = planets[i];
					if (false == planet.IsVisibleFromCamera(cameraGame))
						continue;
					if (true == planet.DisallowSelection)
						continue;

					Vector3 screenPoint = cameraGame.WorldToScreenPoint(planet.transform.position);

					if (screenPoint.x > min.x && screenPoint.x < max.x && screenPoint.y > min.y && screenPoint.y < max.y)
					{
						SelectionManager.Main.Select(planet.GetComponent<SpaceObject>());
					}
				}


			}
			while (false);
			

			// Reset
			m_MouseDragSelectDownPos = null;
			m_MouseDragSelectBoxActivated = false;
			GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			GetComponent<RectTransform>().sizeDelta = Vector2.zero;

			GetComponent<CanvasGroup>().alpha = 0f;
		}

		#endregion Events


		


		
	}
}
