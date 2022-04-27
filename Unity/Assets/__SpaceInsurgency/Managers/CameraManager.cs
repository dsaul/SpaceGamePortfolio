using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class CameraManager : SharedCode.Behaviours.InstanceTracked<CameraManager>
	{
		#region Type / Const

		

		[Serializable]
		[AdvancedInspector]
		public class Preset
		{
			[SerializeField]
			string m_Description;
			[Inspect]
			public string Description
			{
				get { return m_Description; }
				set { m_Description = value; }
			}

			[SerializeField]
			float m_Distance;
			[Inspect]
			public float Distance
			{
				get { return m_Distance; }
				set { m_Distance = value; }
			}

			[SerializeField]
			float m_Tilt;
			[Inspect]
			public float Tilt
			{
				get { return m_Tilt; }
				set { m_Tilt = value; }
			}
		}

		#endregion  Type / Const
		#region Event Definitions

		#endregion Event Definitions
		#region Variables
		
		[SerializeField]
		Camera m_CameraGame;
		[Inspect, Group("Editor")] 
		public Camera CameraGame
		{
			get { return m_CameraGame; }
			set
			{ 
				m_CameraGame = value; 
			}
		}

		[Inspect, Group("Editor")]
		public bool m_CancelFollowOnMove = true;
		[Inspect, Group("Editor")]
		public float m_NormalMoveSpeed = 30f;
		[Inspect, Group("Editor")]
		public float m_FastMoveSpeed = 200f;
		[Inspect, Group("Editor")]
		public float m_RotateSpeed = 600f;
		[Inspect, Group("Editor")]
		public float m_FreeZoomSpeed = 300f;
		[Inspect, Group("Editor")]
		public float m_TiltSpeed = 300f;
		[Inspect, Group("Editor")]
		public float m_PanSpeed = 200f;
		[Inspect, Group("Editor")]
		public bool m_EnableScrollWheelAxisFreeZoom = true;
		[Inspect, Group("Editor")]
		public bool m_AllowScreenEdgeMove = true;
		[Inspect, Group("Editor")]
		public float m_ScreenEdgeBorderWidth = 4f;
		[Inspect, Group("Editor")]
		public bool m_ScreenEdgeMoveBreaksFollow = true;

#warning implement changing of the presets, and saving across loads
		[Inspect, Group("Editor"), Collection(Size = 10, Sortable = true)]
		public Preset[] m_ZoomPresets;
		[Inspect, ReadOnly, Group("Runtime")]
		int m_ZoomPresetUser = 0;

		bool m_IgnoreUserInput = false;

		[Inspect, Group("Runtime")]
		public bool IgnoreUserInput
		{
			get { return m_IgnoreUserInput; }
			set { m_IgnoreUserInput = true; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

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

			SetTimer(1f, true, VectrosityResolutionCheck);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnSubSignal();

			ClearTimer(VectrosityResolutionCheck);
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

			LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;

			//InputManager.Main.OnPanOrbitBegin += MoveGameCameraUp;
			InputManager.Main.OnPanOrbitStay += OnPanOrbitStay;
			//InputManager.Main.OnPanOrbitEnd += MoveGameCameraUp;

			InputManager.Main.OnMoveCameraUpBegin += MoveGameCameraUp;
			InputManager.Main.OnMoveCameraUpStay += MoveGameCameraUp;
			InputManager.Main.OnMoveCameraUpEnd += MoveGameCameraUp;

			InputManager.Main.OnMoveCameraDownBegin += MoveGameCameraDown;
			InputManager.Main.OnMoveCameraDownStay += MoveGameCameraDown;
			InputManager.Main.OnMoveCameraDownEnd += MoveGameCameraDown;

			InputManager.Main.OnMoveCameraLeftBegin += MoveGameCameraLeft;
			InputManager.Main.OnMoveCameraLeftStay += MoveGameCameraLeft;
			InputManager.Main.OnMoveCameraLeftEnd += MoveGameCameraLeft;

			InputManager.Main.OnMoveCameraRightBegin += MoveGameCameraRight;
			InputManager.Main.OnMoveCameraRightStay += MoveGameCameraRight;
			InputManager.Main.OnMoveCameraRightEnd += MoveGameCameraRight;

			InputManager.Main.OnRotateCameraLeftBegin += RotateCameraLeft;
			InputManager.Main.OnRotateCameraLeftStay += RotateCameraLeft;
			InputManager.Main.OnRotateCameraLeftEnd += RotateCameraLeft;

			InputManager.Main.OnRotateCameraRightBegin += RotateCameraRight;
			InputManager.Main.OnRotateCameraRightStay += RotateCameraRight;
			InputManager.Main.OnRotateCameraRightEnd += RotateCameraRight;

			InputManager.Main.OnFreeZoomCameraInBegin += FreeZoomCameraIn;
			InputManager.Main.OnFreeZoomCameraInStay += FreeZoomCameraIn;
			InputManager.Main.OnFreeZoomCameraInEnd += FreeZoomCameraIn;

			InputManager.Main.OnFreeZoomCameraOutBegin += FreeZoomCameraOut;
			InputManager.Main.OnFreeZoomCameraOutStay += FreeZoomCameraOut;
			InputManager.Main.OnFreeZoomCameraOutEnd += FreeZoomCameraOut;

			InputManager.Main.OnTiltCameraUpBegin += TiltCameraUp;
			InputManager.Main.OnTiltCameraUpStay += TiltCameraUp;
			InputManager.Main.OnTiltCameraUpEnd += TiltCameraUp;

			InputManager.Main.OnTiltCameraDownBegin += TiltCameraDown;
			InputManager.Main.OnTiltCameraDownStay += TiltCameraDown;
			InputManager.Main.OnTiltCameraDownEnd += TiltCameraDown;

			InputManager.Main.OnResetCameraToDefaultsEnd += ResetCameraToDefaults;

			InputManager.Main.OnCameraZoomPreset0End += InputManager_OnCameraZoomPreset0End;
			InputManager.Main.OnCameraZoomPreset1End += InputManager_OnCameraZoomPreset1End;
			InputManager.Main.OnCameraZoomPreset2End += InputManager_OnCameraZoomPreset2End;
			InputManager.Main.OnCameraZoomPreset3End += InputManager_OnCameraZoomPreset3End;
			InputManager.Main.OnCameraZoomPreset4End += InputManager_OnCameraZoomPreset4End;
			InputManager.Main.OnCameraZoomPreset5End += InputManager_OnCameraZoomPreset5End;
			InputManager.Main.OnCameraZoomPreset6End += InputManager_OnCameraZoomPreset6End;
			InputManager.Main.OnCameraZoomPreset7End += InputManager_OnCameraZoomPreset7End;
			InputManager.Main.OnCameraZoomPreset8End += InputManager_OnCameraZoomPreset8End;
			InputManager.Main.OnCameraZoomPreset9End += InputManager_OnCameraZoomPreset9End;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == InputManager.Main)
				return;

			LevelGeometry.OnLevelGeometryCreated -= LevelGeometry_OnLevelGeometryCreated;

			//InputManager.Main.OnPanOrbitBegin -= MoveGameCameraUp;
			InputManager.Main.OnPanOrbitStay -= OnPanOrbitStay;
			//InputManager.Main.OnPanOrbitEnd -= MoveGameCameraUp;

			InputManager.Main.OnMoveCameraUpBegin -= MoveGameCameraUp;
			InputManager.Main.OnMoveCameraUpStay -= MoveGameCameraUp;
			InputManager.Main.OnMoveCameraUpEnd -= MoveGameCameraUp;

			InputManager.Main.OnMoveCameraDownBegin -= MoveGameCameraDown;
			InputManager.Main.OnMoveCameraDownStay -= MoveGameCameraDown;
			InputManager.Main.OnMoveCameraDownEnd -= MoveGameCameraDown;

			InputManager.Main.OnMoveCameraLeftBegin -= MoveGameCameraLeft;
			InputManager.Main.OnMoveCameraLeftStay -= MoveGameCameraLeft;
			InputManager.Main.OnMoveCameraLeftEnd -= MoveGameCameraLeft;

			InputManager.Main.OnMoveCameraRightBegin -= MoveGameCameraRight;
			InputManager.Main.OnMoveCameraRightStay -= MoveGameCameraRight;
			InputManager.Main.OnMoveCameraRightEnd -= MoveGameCameraRight;

			InputManager.Main.OnRotateCameraLeftBegin -= RotateCameraLeft;
			InputManager.Main.OnRotateCameraLeftStay -= RotateCameraLeft;
			InputManager.Main.OnRotateCameraLeftEnd -= RotateCameraLeft;

			InputManager.Main.OnRotateCameraRightBegin -= RotateCameraRight;
			InputManager.Main.OnRotateCameraRightStay -= RotateCameraRight;
			InputManager.Main.OnRotateCameraRightEnd -= RotateCameraRight;

			InputManager.Main.OnFreeZoomCameraInBegin -= FreeZoomCameraIn;
			InputManager.Main.OnFreeZoomCameraInStay -= FreeZoomCameraIn;
			InputManager.Main.OnFreeZoomCameraInEnd -= FreeZoomCameraIn;

			InputManager.Main.OnFreeZoomCameraOutBegin -= FreeZoomCameraOut;
			InputManager.Main.OnFreeZoomCameraOutStay -= FreeZoomCameraOut;
			InputManager.Main.OnFreeZoomCameraOutEnd -= FreeZoomCameraOut;

			InputManager.Main.OnResetCameraToDefaultsEnd -= ResetCameraToDefaults;

			InputManager.Main.OnCameraZoomPreset0End -= InputManager_OnCameraZoomPreset0End;
			InputManager.Main.OnCameraZoomPreset1End -= InputManager_OnCameraZoomPreset1End;
			InputManager.Main.OnCameraZoomPreset2End -= InputManager_OnCameraZoomPreset2End;
			InputManager.Main.OnCameraZoomPreset3End -= InputManager_OnCameraZoomPreset3End;
			InputManager.Main.OnCameraZoomPreset4End -= InputManager_OnCameraZoomPreset4End;
			InputManager.Main.OnCameraZoomPreset5End -= InputManager_OnCameraZoomPreset5End;
			InputManager.Main.OnCameraZoomPreset6End -= InputManager_OnCameraZoomPreset6End;
			InputManager.Main.OnCameraZoomPreset7End -= InputManager_OnCameraZoomPreset7End;
			InputManager.Main.OnCameraZoomPreset8End -= InputManager_OnCameraZoomPreset8End;
			InputManager.Main.OnCameraZoomPreset9End -= InputManager_OnCameraZoomPreset9End;

			signalSub = false;
		}

		#endregion Setup
		#region Main

		

		protected override void Update()
		{
			base.Update();

			// Lock the cursor.
#warning have this lock only be when in game and not if in menus
			Cursor.lockState = CursorLockMode.Confined;

			if (false == m_IgnoreUserInput && true == m_EnableScrollWheelAxisFreeZoom)
			{
				if (false == InputManager.Main.IsOverUIElement || true == SpaceInsurgency.Mouseover.DetailText.Main.IsPointerOver)
					m_CameraGame.GetComponent<RtsCamera>().Distance -= cInput.GetAxisRaw("Free Zoom Axis") * (m_FreeZoomSpeed * 10) * Time.deltaTime;
			}
			


			// Screen Edge Movement
			if (false == Application.isEditor &&  m_AllowScreenEdgeMove && (false == m_CameraGame.GetComponent<RtsCamera>().IsFollowing || m_ScreenEdgeMoveBreaksFollow))
			{
				bool hasMovement = false;

				if (Input.mousePosition.y > (Screen.height - m_ScreenEdgeBorderWidth))
				{
					hasMovement = true;
					m_CameraGame.GetComponent<RtsCamera>().AddToPosition(0, 0, m_NormalMoveSpeed * Time.deltaTime);
				}
				else if (Input.mousePosition.y < m_ScreenEdgeBorderWidth)
				{
					hasMovement = true;
					m_CameraGame.GetComponent<RtsCamera>().AddToPosition(0, 0, -1 * m_NormalMoveSpeed * Time.deltaTime);
				}

				if (Input.mousePosition.x > (Screen.width - m_ScreenEdgeBorderWidth))
				{
					hasMovement = true;
					m_CameraGame.GetComponent<RtsCamera>().AddToPosition(m_NormalMoveSpeed * Time.deltaTime, 0, 0);
				}
				else if (Input.mousePosition.x < m_ScreenEdgeBorderWidth)
				{
					hasMovement = true;
					m_CameraGame.GetComponent<RtsCamera>().AddToPosition(-1 * m_NormalMoveSpeed * Time.deltaTime, 0, 0);
				}

				if (hasMovement && m_CameraGame.GetComponent<RtsCamera>().IsFollowing && m_ScreenEdgeMoveBreaksFollow)
				{
					m_CameraGame.GetComponent<RtsCamera>().EndFollow();
				}
			}
		}

		#endregion Main
		#region Zoom Presets

		public void SwitchToZoomPreset(int Index, bool UserInitiated = true)
		{
			m_CameraGame.GetComponent<RtsCamera>().Distance = m_ZoomPresets[Index].Distance;
			m_CameraGame.GetComponent<RtsCamera>().Tilt = m_ZoomPresets[Index].Tilt;

			if (true == UserInitiated)
			{
				m_ZoomPresetUser = Index;
			}
		}

		#endregion Zoom Presets
		#region Events

		void LevelGeometry_OnLevelGeometryCreated(LevelGeometry source, bool fromSaveFile)
		{
			RtsCamera rtsCamera = m_CameraGame.GetComponent<RtsCamera>();
			rtsCamera.MinDistance = source.MinDistance;
			rtsCamera.MaxDistance = source.MaxDistance;
			rtsCamera.MinTilt = source.MinTilt;
			rtsCamera.MaxTilt = source.MaxTilt;
		}


		void OnPanOrbitStay(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			if (true == source.ModifierKeyActive)
				Pan(source);
			else
				Orbit(source);
		}

		void Pan(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			var panX = -1 * cInput.GetAxisRaw(InputManager.kAxisMouseX) * m_PanSpeed * Time.deltaTime;
			var panZ = -1 * cInput.GetAxisRaw(InputManager.kAxisMouseY) * m_PanSpeed * Time.deltaTime;

			m_CameraGame.GetComponent<RtsCamera>().AddToPosition(panX, 0, panZ);

			if (true == m_CancelFollowOnMove && (Mathf.Abs(panX) > 0.001f || Mathf.Abs(panZ) > 0.001f))
			{
				m_CameraGame.GetComponent<RtsCamera>().EndFollow();
			}
		}

		void Orbit(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			float tilt = cInput.GetAxisRaw(InputManager.kAxisMouseY);
			m_CameraGame.GetComponent<RtsCamera>().Tilt -= tilt * (m_TiltSpeed * 2) * Time.deltaTime;

			float rot = cInput.GetAxisRaw(InputManager.kAxisMouseX);
			m_CameraGame.GetComponent<RtsCamera>().Rotation += rot * (m_RotateSpeed) * Time.deltaTime;
		}

		void MoveGameCameraUp(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			float speed = InputManager.Main.ModifierKeyActive ? m_FastMoveSpeed : m_NormalMoveSpeed;
			m_CameraGame.GetComponent<RtsCamera>().AddToPosition(0, 0, speed * Time.deltaTime);

			if (true == m_CancelFollowOnMove && true == m_CameraGame.GetComponent<RtsCamera>().IsFollowing)
				m_CameraGame.GetComponent<RtsCamera>().EndFollow();
		}

		void MoveGameCameraDown(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			float speed = InputManager.Main.ModifierKeyActive ? m_FastMoveSpeed : m_NormalMoveSpeed;
			m_CameraGame.GetComponent<RtsCamera>().AddToPosition(0f, 0f, -(speed * Time.deltaTime));

			if (true == m_CancelFollowOnMove && true == m_CameraGame.GetComponent<RtsCamera>().IsFollowing)
				m_CameraGame.GetComponent<RtsCamera>().EndFollow();
		}
		void MoveGameCameraLeft(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			float speed = InputManager.Main.ModifierKeyActive ? m_FastMoveSpeed : m_NormalMoveSpeed;
			m_CameraGame.GetComponent<RtsCamera>().AddToPosition(-(speed * Time.deltaTime), 0f, 0f);

			if (true == m_CancelFollowOnMove && true == m_CameraGame.GetComponent<RtsCamera>().IsFollowing)
				m_CameraGame.GetComponent<RtsCamera>().EndFollow();
		}
		void MoveGameCameraRight(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			float speed = InputManager.Main.ModifierKeyActive ? m_FastMoveSpeed : m_NormalMoveSpeed;
			m_CameraGame.GetComponent<RtsCamera>().AddToPosition(speed * Time.deltaTime, 0f, 0f);

			if (true == m_CancelFollowOnMove && true == m_CameraGame.GetComponent<RtsCamera>().IsFollowing)
				m_CameraGame.GetComponent<RtsCamera>().EndFollow();
		}

		void RotateCameraLeft(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			m_CameraGame.GetComponent<RtsCamera>().Rotation += m_RotateSpeed * Time.deltaTime;
		}

		void RotateCameraRight(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			m_CameraGame.GetComponent<RtsCamera>().Rotation -= m_RotateSpeed * Time.deltaTime;
		}

		void FreeZoomCameraIn(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			m_CameraGame.GetComponent<RtsCamera>().Distance -= m_FreeZoomSpeed * Time.deltaTime;
		}

		void FreeZoomCameraOut(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			m_CameraGame.GetComponent<RtsCamera>().Distance += m_FreeZoomSpeed * Time.deltaTime;
		}

		void TiltCameraUp(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			m_CameraGame.GetComponent<RtsCamera>().Tilt += m_TiltSpeed * Time.deltaTime;
		}

		void TiltCameraDown(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;

			m_CameraGame.GetComponent<RtsCamera>().Tilt -= m_TiltSpeed * Time.deltaTime;
		}

		void ResetCameraToDefaults(InputManager source)
		{
			if (true == m_IgnoreUserInput)
				return;
			
			m_CameraGame.GetComponent<RtsCamera>().ResetToInitialValues(false, false);
		}

		void InputManager_OnCameraZoomPreset0End(InputManager source)
		{
			SwitchToZoomPreset(0);
		}

		void InputManager_OnCameraZoomPreset1End(InputManager source)
		{
			SwitchToZoomPreset(1);
		}

		void InputManager_OnCameraZoomPreset2End(InputManager source)
		{
			SwitchToZoomPreset(2);
		}

		void InputManager_OnCameraZoomPreset3End(InputManager source)
		{
			SwitchToZoomPreset(3);
		}

		void InputManager_OnCameraZoomPreset4End(InputManager source)
		{
			SwitchToZoomPreset(4);
		}

		void InputManager_OnCameraZoomPreset5End(InputManager source)
		{
			SwitchToZoomPreset(5);
		}

		void InputManager_OnCameraZoomPreset6End(InputManager source)
		{
			SwitchToZoomPreset(6);
		}

		void InputManager_OnCameraZoomPreset7End(InputManager source)
		{
			SwitchToZoomPreset(7);
		}

		void InputManager_OnCameraZoomPreset8End(InputManager source)
		{
			SwitchToZoomPreset(8);
		}

		void InputManager_OnCameraZoomPreset9End(InputManager source)
		{
			SwitchToZoomPreset(9);
		}


		#endregion Events
		#region Utility

		public void FocusPlayerFleetOrRandomStaticAgent()
		{
			if (0 == Faction.PlayerFaction.DynamicAgents.Count)
			{
				// Focus a random planet.
				StaticAgent randomPlanet = StaticAgent.Random;
				m_CameraGame.GetComponent<RtsCamera>().JumpTo(randomPlanet.GetNewIdleAndSpawnPoint(), true);
			}
			else
			{
				// Focus the player's ships.
				Faction.PlayerFaction.CentreCameraOnDynamicAgents();
			}
		}

		#endregion Utility
		#region Vectrosity

		float lastWidth = float.MaxValue;
		float lastHeight = float.MaxValue;

		[ContextMenu("VectrosityResolutionCheck()")]
		void VectrosityResolutionCheck()
		{
			float curWidth = Screen.width;
			float curHeight = Screen.height;

			if (curWidth != lastWidth || curHeight != lastHeight)
			{
				VectorLine.SetCamera3D(CameraManager.Main.CameraGame);
				lastWidth = curWidth;
				lastHeight = curHeight;
			}
		}

		#endregion Vectrosity
	}
}