using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class ActionsCanvasItem : SharedCode.Behaviours.Base
	{
		#region Delegates

		public Action OnClickAction;
		public Func<bool> CheckEnableFunc; // returns is interactable

		#endregion Delegates
		#region Variables

		bool m_TargetSelectActive = false;
		[Inspect]
		public bool TargetSelectActive
		{
			get { return m_TargetSelectActive; }
		}

		bool m_DoTargetSelectFirst = false;
		[Inspect]
		public bool DoTargetSelectFirst
		{
			get { return m_DoTargetSelectFirst; }
			set { m_DoTargetSelectFirst = value; }
		}

		bool m_IsStatic = false;
		[Inspect]
		public bool IsStatic
		{
			get { return m_IsStatic; }
			set { m_IsStatic = value; }
		}

		#endregion Variables




		
		
		public void ClickCallback()
		{
			// If target select is currently active we don't want to do anything else.
			if (true == TargetSelectActive)
				return;

			// If we need to be over a target before the action happens set the mode to targeting and set the targeting cursor.
			if (true == DoTargetSelectFirst && InputManager.Main.CurrentInputMode != InputManager.InputMode.Targeting)
			{
				InputManager.Main.CurrentInputMode = InputManager.InputMode.Targeting;
				Cursor.SetCursor(InputManager.Main.TargetCursor, InputManager.Main.TargetCursorHotspot, CursorMode.Auto);

				// We need to wait for a fixed update before we listen for mouse up events because this click is a mouse up!
				TimeManager.Main.OnTimeTickFixedUpdate += ActivateTargetSelect;
				
				return;
			}
			
			if (false == DoTargetSelectFirst)
			{
				if (null != OnClickAction)
					OnClickAction();
				return;
			}
		}

		void ActivateTargetSelect()
		{
			TimeManager.Main.OnTimeTickFixedUpdate -= ActivateTargetSelect;
			m_TargetSelectActive = true;
		}

		protected override void Update()
		{
			base.Update();

			if (true == m_TargetSelectActive && true == Input.GetKeyDown(KeyCode.Escape))
			{
				m_TargetSelectActive = false;
				InputManager.Main.CurrentInputMode = InputManager.InputMode.Game;
				Cursor.SetCursor(InputManager.Main.DefaultCursor, InputManager.Main.DefaultCursorHotspot, CursorMode.Auto);
			}
			else if (true == m_TargetSelectActive && true == Input.GetMouseButtonUp(0))
			{
				m_TargetSelectActive = false;

				OnClickAction();

				TimeManager.Main.OnTimeTickFixedUpdate += SetInputModeToGameAfterFixedUpdate;
			}
		}

		void SetInputModeToGameAfterFixedUpdate()
		{
			TimeManager.Main.OnTimeTickFixedUpdate -= SetInputModeToGameAfterFixedUpdate;
			InputManager.Main.CurrentInputMode = InputManager.InputMode.Game;
			Cursor.SetCursor(InputManager.Main.DefaultCursor, InputManager.Main.DefaultCursorHotspot, CursorMode.Auto);
		}




		

		

		

		

		protected override void OnEnable()
		{
			base.OnEnable();

			if (null != SelectionManager.Main)
			{
				SelectionManager.Main.OnSelectionAdded += SelectionManager_OnSelectionAdded;
				SelectionManager.Main.OnSelectionRemoved += SelectionManager_OnSelectionRemoved;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (null != SelectionManager.Main)
			{
				SelectionManager.Main.OnSelectionAdded -= SelectionManager_OnSelectionAdded;
				SelectionManager.Main.OnSelectionRemoved -= SelectionManager_OnSelectionRemoved;
			}
		}

		void SelectionManager_OnSelectionAdded(SelectionManager sender, SpaceObject obj)
		{
			SelectionManager_OnSelectionChanged();
		}

		void SelectionManager_OnSelectionRemoved(SelectionManager sender, SpaceObject obj)
		{
			SelectionManager_OnSelectionChanged();
		}

		void SelectionManager_OnSelectionChanged()
		{
			CheckInteractableNow();
		}

		[Inspect]
		public void CheckInteractableNow()
		{
			GetComponent<UnityEngine.UI.Button>().interactable = CheckEnableFunc();
		}
	}
}