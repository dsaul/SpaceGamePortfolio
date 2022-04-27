#define DEBUG

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vectrosity;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class InputManager : SharedCode.Behaviours.InstanceTracked<InputManager>
	{
		#region Types / Const

		public enum InputMode
		{
			Unspecified = 0,
			Targeting,
			Skillshot,
			Game,
			Menu
		};

		public const string kButtonPrimary = "Primary";
		public const string kButtonSecondary = "Secondary";
		public const string kButtonPanOrbit = "Pan / Orbit";

		public const string kButtonModifier = "Modifier Key";

		public const string kButtonDebugSpawnFriendlyShip = "DebugSpawnFriendlyShip";
		public const string kButtonDebugSpawnHostileShip = "DebugSpawnHostileShip";

		public const string kButtonMoveCameraUp = "Move Camera Up";
		public const string kButtonMoveCameraDown = "Move Camera Down";
		public const string kButtonMoveCameraLeft = "Move Camera Left";
		public const string kButtonMoveCameraRight = "Move Camera Right";

		public const string kButtonRotateCameraLeft = "Rotate Camera Left";
		public const string kButtonRotateCameraRight = "Rotate Camera Right";

		public const string kButtonFreeZoomCameraIn = "Free Zoom In (Button)";
		public const string kButtonFreeZoomCameraOut = "Free Zoom Out (Button)";

		public const string kButtonTiltCameraUp = "Tilt Camera Up";
		public const string kButtonTiltCameraDown = "Tilt Camera Down";

		public const string kButtonResetCameraToDefaults = "Reset Camera to Defaults";

		public const string kButtonFreeZoomAxisPositive = "Free Zoom In (Axis)";
		public const string kButtonFreeZoomAxisNegative = "Free Zoom Out (Axis)";
		public const string kFreeZoomAxis = "Free Zoom Axis";

		public const string kAxisMouseXNegative = "Mouse X Axis (Negative)";
		public const string kAxisMouseXPositive = "Mouse X Axis (Positive)";
		public const string kAxisMouseX = "Mouse X Axis";

		public const string kAxisMouseYNegative = "Mouse Y Axis (Negative)";
		public const string kAxisMouseYPositive = "Mouse Y Axis (Positive)";
		public const string kAxisMouseY = "Mouse Y Axis";

		public const string kButtonZoomPreset0 = "Zoom Preset 0";
		public const string kButtonZoomPreset1 = "Zoom Preset 1";
		public const string kButtonZoomPreset2 = "Zoom Preset 2";
		public const string kButtonZoomPreset3 = "Zoom Preset 3";
		public const string kButtonZoomPreset4 = "Zoom Preset 4";
		public const string kButtonZoomPreset5 = "Zoom Preset 5";
		public const string kButtonZoomPreset6 = "Zoom Preset 6";
		public const string kButtonZoomPreset7 = "Zoom Preset 7";
		public const string kButtonZoomPreset8 = "Zoom Preset 8";
		public const string kButtonZoomPreset9 = "Zoom Preset 9";

		#endregion Types / Const
		#region Event Definitions

		public event Action<InputManager, InputMode> OnInputModeChanged; // InputManager source, InputMode newInputMode

		public event Action<InputManager> OnPrimaryBegin; // InputManager source
		public event Action<InputManager> OnPrimaryStay; // InputManager source
		public event Action<InputManager> OnPrimaryEnd; // InputManager source

		public event Action<InputManager> OnPanOrbitBegin; // InputManager source
		public event Action<InputManager> OnPanOrbitStay; // InputManager source
		public event Action<InputManager> OnPanOrbitEnd; // InputManager source

		public event Action<InputManager> OnSecondaryBegin; // InputManager source
		public event Action<InputManager> OnSecondaryStay; // InputManager source
		public event Action<InputManager> OnSecondaryEnd; // InputManager source

		public event Action<InputManager> OnMoveCameraUpBegin; // InputManager source
		public event Action<InputManager> OnMoveCameraUpStay; // InputManager source
		public event Action<InputManager> OnMoveCameraUpEnd; // InputManager source

		public event Action<InputManager> OnMoveCameraDownBegin; // InputManager source
		public event Action<InputManager> OnMoveCameraDownStay; // InputManager source
		public event Action<InputManager> OnMoveCameraDownEnd; // InputManager source

		public event Action<InputManager> OnMoveCameraLeftBegin; // InputManager source
		public event Action<InputManager> OnMoveCameraLeftStay; // InputManager source
		public event Action<InputManager> OnMoveCameraLeftEnd; // InputManager source

		public event Action<InputManager> OnMoveCameraRightBegin; // InputManager source
		public event Action<InputManager> OnMoveCameraRightStay; // InputManager source
		public event Action<InputManager> OnMoveCameraRightEnd; // InputManager source

		public event Action<InputManager> OnRotateCameraLeftBegin; // InputManager source
		public event Action<InputManager> OnRotateCameraLeftStay; // InputManager source
		public event Action<InputManager> OnRotateCameraLeftEnd; // InputManager source

		public event Action<InputManager> OnRotateCameraRightBegin; // InputManager source
		public event Action<InputManager> OnRotateCameraRightStay; // InputManager source
		public event Action<InputManager> OnRotateCameraRightEnd; // InputManager source

		public event Action<InputManager> OnFreeZoomCameraInBegin; // InputManager source
		public event Action<InputManager> OnFreeZoomCameraInStay; // InputManager source
		public event Action<InputManager> OnFreeZoomCameraInEnd; // InputManager source

		public event Action<InputManager> OnFreeZoomCameraOutBegin; // InputManager source
		public event Action<InputManager> OnFreeZoomCameraOutStay; // InputManager source
		public event Action<InputManager> OnFreeZoomCameraOutEnd; // InputManager source

		public event Action<InputManager> OnTiltCameraUpBegin; // InputManager source
		public event Action<InputManager> OnTiltCameraUpStay; // InputManager source
		public event Action<InputManager> OnTiltCameraUpEnd; // InputManager source

		public event Action<InputManager> OnTiltCameraDownBegin; // InputManager source
		public event Action<InputManager> OnTiltCameraDownStay; // InputManager source
		public event Action<InputManager> OnTiltCameraDownEnd; // InputManager source

		public event Action<InputManager> OnResetCameraToDefaultsBegin; // InputManager source
		public event Action<InputManager> OnResetCameraToDefaultsStay; // InputManager source
		public event Action<InputManager> OnResetCameraToDefaultsEnd; // InputManager source

		public event Action<InputManager, List<SpaceObject>> OnMouseoverSpaceObjectExactChanged; // InputManager source, List<SpaceObject> objects
		public event Action<InputManager, List<SpaceObject>> OnMouseoverSpaceObjectFudgedChanged; // InputManager source, List<SpaceObject> objects

		public event Action<InputManager> OnCameraZoomPreset0Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset0Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset0End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset1Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset1Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset1End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset2Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset2Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset2End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset3Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset3Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset3End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset4Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset4Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset4End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset5Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset5Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset5End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset6Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset6Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset6End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset7Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset7Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset7End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset8Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset8Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset8End; // InputManager source

		public event Action<InputManager> OnCameraZoomPreset9Begin; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset9Stay; // InputManager source
		public event Action<InputManager> OnCameraZoomPreset9End; // InputManager source

		#endregion Event Definitions
		#region Variables

		[SerializeField]
		Texture2D m_DefaultCursor;
		[Inspect, Group("Editor")]
		public Texture2D DefaultCursor
		{
			get { return m_DefaultCursor; }
			set { m_DefaultCursor = value; }
		}

		[SerializeField]
		Vector2 m_DefaultCursorHotspot;
		[Inspect, Group("Editor")]
		public Vector2 DefaultCursorHotspot
		{
			get { return m_DefaultCursorHotspot; }
			set { m_DefaultCursorHotspot = value; }
		}

		[SerializeField]
		Texture2D m_TargetCursor;
		[Inspect, Group("Editor")]
		public Texture2D TargetCursor
		{
			get { return m_TargetCursor; }
			set { m_TargetCursor = value; }
		}

		[SerializeField]
		Vector2 m_TargetCursorHotspot;
		[Inspect, Group("Editor")]
		public Vector2 TargetCursorHotspot
		{
			get { return m_TargetCursorHotspot; }
			set { m_TargetCursorHotspot = value; }
		}

		[SerializeField]
		GameObject m_RTS3DArrowPrefab;
		[Inspect, Group("Editor")]
		public GameObject RTS3DArrowPrefab
		{
			get { return m_RTS3DArrowPrefab; }
			set { m_RTS3DArrowPrefab = value; }
		}


		InputMode m_CurrentInputMode = InputMode.Unspecified;
		[Inspect, Group("Runtime"), ReadOnly]
		public InputMode CurrentInputMode
		{
			get { return m_CurrentInputMode; }
			set
			{
				if (false == Application.isPlaying)
					return;
				OnInputModeChanged(this, value);
			}
		}

		[SerializeField]
		float m_DoubleClickThreshold = 0.5f;

		[Inspect, Group("Editor")]
		public float DoubleClickThreshold
		{
			get { return m_DoubleClickThreshold; }
			set { m_DoubleClickThreshold = value; }
		}

		float m_PrimaryButtonEndTime = float.MinValue;

		[Inspect, Group("Runtime"), ReadOnly]
		public float PrimaryButtonEndTime
		{
			get { return m_PrimaryButtonEndTime; }
			private set
			{
				if (false == Application.isPlaying)
					return;
				m_PrimaryButtonEndTime = value;
			}
		}

		int m_PrimaryDoubleClickFrame = int.MinValue;

		[Inspect, Group("Runtime"), ReadOnly]
		public int PrimaryDoubleClickFrame
		{
			get { return m_PrimaryDoubleClickFrame; }
			private set { m_PrimaryDoubleClickFrame = value; }
		}

		#endregion Variables
		#region Ancessors

		[Inspect, Group("Runtime")]
		public bool IsOverUIElement
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				return EventSystem.current.IsPointerOverGameObject();
			}
		}

		[Inspect, Group("Runtime")]
		public bool IsLeftDoubleClickFrame
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				return m_PrimaryDoubleClickFrame == Time.frameCount;
			}
		}

		[Inspect, Group("Runtime")]
		public bool ModifierKeyActive
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				return true == cInput.GetKey(kButtonModifier);
			}
		}


		List<SpaceObject> m_MouseOverSpaceObjectsFudged;

		[Inspect, Group("Runtime"), ReadOnly]
		public List<SpaceObject> MouseOverSpaceObjectsFudged
		{
			get { return m_MouseOverSpaceObjectsFudged; }
			private set { m_MouseOverSpaceObjectsFudged = value; }
		}

		Vector3 m_MouseMovementPlaneLocationFudged;

		[Inspect, Group("Runtime"), ReadOnly]
		public Vector3 MouseMovementPlaneLocationFudged
		{
			get { return m_MouseMovementPlaneLocationFudged; }
			set { m_MouseMovementPlaneLocationFudged = value; }
		}

		List<SpaceObject> m_MouseOverSpaceObjectsExact;

		[Inspect, Group("Runtime"), ReadOnly]
		public List<SpaceObject> MouseOverSpaceObjectsExact
		{
			get { return m_MouseOverSpaceObjectsExact; }
			private set { m_MouseOverSpaceObjectsExact = value; }
		}

		Vector3 m_MouseMovementPlaneLocationExact;

		[Inspect, Group("Runtime"), ReadOnly]
		public Vector3 MouseMovementPlaneLocationExact
		{
			get { return m_MouseMovementPlaneLocationExact; }
			set { m_MouseMovementPlaneLocationExact = value; }
		}

		
		
		#endregion Ancessors
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			OnInputModeChanged = new Action<InputManager, InputMode>(delegate(InputManager source, InputMode newInputMode) {
				m_CurrentInputMode = newInputMode;
			});

			OnPrimaryBegin = new Action<InputManager>(delegate { });
			OnPrimaryStay = new Action<InputManager>(delegate { });
			OnPrimaryEnd = new Action<InputManager>(delegate {
				if (m_PrimaryButtonEndTime > (Time.time - DoubleClickThreshold))
					m_PrimaryDoubleClickFrame = Time.frameCount;
				m_PrimaryButtonEndTime = Time.time;
			});

			OnSecondaryBegin = new Action<InputManager>(delegate { });
			OnSecondaryStay = new Action<InputManager>(delegate { });
			OnSecondaryEnd = new Action<InputManager>(delegate { });

			OnPanOrbitBegin = new Action<InputManager>(delegate { });
			OnPanOrbitStay = new Action<InputManager>(delegate { });
			OnPanOrbitEnd = new Action<InputManager>(delegate { });

			OnMoveCameraUpBegin = new Action<InputManager>(delegate { });
			OnMoveCameraUpStay = new Action<InputManager>(delegate { });
			OnMoveCameraUpEnd = new Action<InputManager>(delegate { });

			OnMoveCameraDownBegin = new Action<InputManager>(delegate { });
			OnMoveCameraDownStay = new Action<InputManager>(delegate { });
			OnMoveCameraDownEnd = new Action<InputManager>(delegate { });

			OnMoveCameraLeftBegin = new Action<InputManager>(delegate { });
			OnMoveCameraLeftStay = new Action<InputManager>(delegate { });
			OnMoveCameraLeftEnd = new Action<InputManager>(delegate { });

			OnMoveCameraRightBegin = new Action<InputManager>(delegate { });
			OnMoveCameraRightStay = new Action<InputManager>(delegate { });
			OnMoveCameraRightEnd = new Action<InputManager>(delegate { });

			OnRotateCameraLeftBegin = new Action<InputManager>(delegate { });
			OnRotateCameraLeftStay = new Action<InputManager>(delegate { });
			OnRotateCameraLeftEnd = new Action<InputManager>(delegate { });

			OnRotateCameraRightBegin = new Action<InputManager>(delegate { });
			OnRotateCameraRightStay = new Action<InputManager>(delegate { });
			OnRotateCameraRightEnd = new Action<InputManager>(delegate { });

			OnFreeZoomCameraInBegin = new Action<InputManager>(delegate { });
			OnFreeZoomCameraInStay = new Action<InputManager>(delegate { });
			OnFreeZoomCameraInEnd = new Action<InputManager>(delegate { });

			OnFreeZoomCameraOutBegin = new Action<InputManager>(delegate { });
			OnFreeZoomCameraOutStay = new Action<InputManager>(delegate { });
			OnFreeZoomCameraOutEnd = new Action<InputManager>(delegate { });

			OnTiltCameraUpBegin = new Action<InputManager>(delegate { });
			OnTiltCameraUpStay = new Action<InputManager>(delegate { });
			OnTiltCameraUpEnd = new Action<InputManager>(delegate { });

			OnTiltCameraDownBegin = new Action<InputManager>(delegate { });
			OnTiltCameraDownStay = new Action<InputManager>(delegate { });
			OnTiltCameraDownEnd = new Action<InputManager>(delegate { });

			OnResetCameraToDefaultsBegin = new Action<InputManager>(delegate { });
			OnResetCameraToDefaultsStay = new Action<InputManager>(delegate { });
			OnResetCameraToDefaultsEnd = new Action<InputManager>(delegate { });

			m_MouseOverSpaceObjectsFudgedTmp = new List<SpaceObject>();
			m_MouseOverSpaceObjectsExactTmp = new List<SpaceObject>();
			m_MouseOverSpaceObjectsFudged = new List<SpaceObject>();
			m_MouseOverSpaceObjectsExact = new List<SpaceObject>();
			OnMouseoverSpaceObjectExactChanged = new Action<InputManager, List<SpaceObject>>(delegate { });
			OnMouseoverSpaceObjectFudgedChanged = new Action<InputManager, List<SpaceObject>>(delegate { });

			OnCameraZoomPreset0Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset0Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset0End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset1Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset1Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset1End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset2Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset2Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset2End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset3Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset3Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset3End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset4Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset4Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset4End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset5Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset5Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset5End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset6Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset6Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset6End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset7Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset7Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset7End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset8Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset8Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset8End = new Action<InputManager>(delegate { });

			OnCameraZoomPreset9Begin = new Action<InputManager>(delegate { });
			OnCameraZoomPreset9Stay = new Action<InputManager>(delegate { });
			OnCameraZoomPreset9End = new Action<InputManager>(delegate { });
		}


		protected override void Start()
		{
			base.Start();

			cInput.SetKey(kButtonDebugSpawnFriendlyShip, Keys.N);
			cInput.SetKey(kButtonDebugSpawnHostileShip, Keys.M);
			cInput.SetKey(kButtonModifier, Keys.LeftShift, Keys.RightShift);

			cInput.SetKey(kButtonPrimary, Keys.Mouse0);
			cInput.SetKey(kButtonSecondary, Keys.Mouse1);
			cInput.SetKey(kButtonPanOrbit, Keys.Mouse2);

			cInput.SetKey(kButtonMoveCameraUp, Keys.W, Keys.UpArrow);
			cInput.SetKey(kButtonMoveCameraDown, Keys.S, Keys.DownArrow);
			cInput.SetKey(kButtonMoveCameraLeft, Keys.A, Keys.LeftArrow);
			cInput.SetKey(kButtonMoveCameraRight, Keys.D, Keys.RightArrow);

			cInput.SetKey(kButtonRotateCameraLeft, Keys.Q);
			cInput.SetKey(kButtonRotateCameraRight, Keys.E);

			cInput.SetKey(kButtonTiltCameraUp, Keys.R);
			cInput.SetKey(kButtonTiltCameraDown, Keys.F);

			cInput.SetKey(kButtonResetCameraToDefaults, Keys.Backslash);

			cInput.SetKey(kButtonFreeZoomCameraIn, Keys.RightBracket);
			cInput.SetKey(kButtonFreeZoomCameraOut, Keys.LeftBracket);

			cInput.SetKey(kButtonFreeZoomAxisPositive, Keys.MouseWheelUp);
			cInput.SetKey(kButtonFreeZoomAxisNegative, Keys.MouseWheelDown);
			cInput.SetAxis(kFreeZoomAxis, kButtonFreeZoomAxisNegative, kButtonFreeZoomAxisPositive);

			cInput.SetKey(kAxisMouseXNegative, Keys.MouseLeft);
			cInput.SetKey(kAxisMouseXPositive, Keys.MouseRight);
			cInput.SetAxis(kAxisMouseX, kAxisMouseXNegative, kAxisMouseXPositive);

			cInput.SetKey(kAxisMouseYPositive, Keys.MouseUp);
			cInput.SetKey(kAxisMouseYNegative, Keys.MouseDown);
			cInput.SetAxis(kAxisMouseY, kAxisMouseYNegative, kAxisMouseYPositive);

			cInput.SetKey(kButtonZoomPreset0, Keys.Keypad0);
			cInput.SetKey(kButtonZoomPreset1, Keys.Keypad1);
			cInput.SetKey(kButtonZoomPreset2, Keys.Keypad2);
			cInput.SetKey(kButtonZoomPreset3, Keys.Keypad3);
			cInput.SetKey(kButtonZoomPreset4, Keys.Keypad4);
			cInput.SetKey(kButtonZoomPreset5, Keys.Keypad5);
			cInput.SetKey(kButtonZoomPreset6, Keys.Keypad6);
			cInput.SetKey(kButtonZoomPreset7, Keys.Keypad7);
			cInput.SetKey(kButtonZoomPreset8, Keys.Keypad8);
			cInput.SetKey(kButtonZoomPreset9, Keys.Keypad9);
		}

		

		#endregion Setup
		#region Main

		List<SpaceObject> m_MouseOverSpaceObjectsExactTmp;
		List<SpaceObject> m_MouseOverSpaceObjectsFudgedTmp;

		protected override void Update()
		{
			base.Update();

			// Events for when we mouse over things.
			bool exactChanged = false;
			bool fudgedChanged = false;

			m_MouseOverSpaceObjectsExactTmp.Clear();
			m_MouseOverSpaceObjectsFudgedTmp.Clear();

			LevelGeometry.RaycastObjectsAndMovementLocation(ref m_MouseOverSpaceObjectsExactTmp, out m_MouseMovementPlaneLocationExact, CameraManager.Main.CameraGame);
			LevelGeometry.SpherecastObjectsAndMovementLocation(ref m_MouseOverSpaceObjectsFudgedTmp, out m_MouseMovementPlaneLocationFudged, CameraManager.Main.CameraGame);

			if (false == m_MouseOverSpaceObjectsExactTmp.SequenceEqual<SpaceObject>(m_MouseOverSpaceObjectsExact))
				exactChanged = true;

			if (false == m_MouseOverSpaceObjectsFudgedTmp.SequenceEqual<SpaceObject>(m_MouseOverSpaceObjectsFudged))
				fudgedChanged = true;

			m_MouseOverSpaceObjectsExact.Clear();
			m_MouseOverSpaceObjectsExact.AddAll<SpaceObject>(m_MouseOverSpaceObjectsExactTmp);

			m_MouseOverSpaceObjectsFudged.Clear();
			m_MouseOverSpaceObjectsFudged.AddAll<SpaceObject>(m_MouseOverSpaceObjectsFudgedTmp);

			if (true == exactChanged)
				OnMouseoverSpaceObjectExactChanged(this, m_MouseOverSpaceObjectsExact);
			if (true == fudgedChanged)
				OnMouseoverSpaceObjectFudgedChanged(this, m_MouseOverSpaceObjectsFudged);

			// kButtonPrimary
			if (true == cInput.GetButtonDown(kButtonPrimary))
				OnPrimaryBegin(this);
			if (true == cInput.GetButton(kButtonPrimary))
				OnPrimaryStay(this);
			if (true == cInput.GetButtonUp(kButtonPrimary))
				OnPrimaryEnd(this);

			// kButtonSecondary
			if (true == cInput.GetButtonDown(kButtonSecondary))
				OnSecondaryBegin(this);
			if (true == cInput.GetButton(kButtonSecondary))
				OnSecondaryStay(this);
			if (true == cInput.GetButtonUp(kButtonSecondary))
				OnSecondaryEnd(this);

			// kButtonPanOrbit
			if (true == cInput.GetButtonDown(kButtonPanOrbit))
				OnPanOrbitBegin(this);
			if (true == cInput.GetButton(kButtonPanOrbit))
				OnPanOrbitStay(this);
			if (true == cInput.GetButtonUp(kButtonPanOrbit))
				OnPanOrbitEnd(this);

			// kButtonMoveCameraUp
			if (true == cInput.GetButtonDown(kButtonMoveCameraUp))
				OnMoveCameraUpBegin(this);
			if (true == cInput.GetButton(kButtonMoveCameraUp))
				OnMoveCameraUpStay(this);
			if (true == cInput.GetButtonUp(kButtonMoveCameraUp))
				OnMoveCameraUpEnd(this);

			// kButtonMoveCameraDown
			if (true == cInput.GetButtonDown(kButtonMoveCameraDown))
				OnMoveCameraDownBegin(this);
			if (true == cInput.GetButton(kButtonMoveCameraDown))
				OnMoveCameraDownStay(this);
			if (true == cInput.GetButtonUp(kButtonMoveCameraDown))
				OnMoveCameraDownEnd(this);

			// kButtonMoveCameraLeft
			if (true == cInput.GetButtonDown(kButtonMoveCameraLeft))
				OnMoveCameraLeftBegin(this);
			if (true == cInput.GetButton(kButtonMoveCameraLeft))
				OnMoveCameraLeftStay(this);
			if (true == cInput.GetButtonUp(kButtonMoveCameraLeft))
				OnMoveCameraLeftEnd(this);

			// kButtonMoveCameraRight
			if (true == cInput.GetButtonDown(kButtonMoveCameraRight))
				OnMoveCameraRightBegin(this);
			if (true == cInput.GetButton(kButtonMoveCameraRight))
				OnMoveCameraRightStay(this);
			if (true == cInput.GetButtonUp(kButtonMoveCameraRight))
				OnMoveCameraRightEnd(this);

			// kButtonRotateCameraLeft
			if (true == cInput.GetButtonDown(kButtonRotateCameraLeft))
				OnRotateCameraLeftBegin(this);
			if (true == cInput.GetButton(kButtonRotateCameraLeft))
				OnRotateCameraLeftStay(this);
			if (true == cInput.GetButtonUp(kButtonRotateCameraLeft))
				OnRotateCameraLeftEnd(this);

			// kButtonRotateCameraRight
			if (true == cInput.GetButtonDown(kButtonRotateCameraRight))
				OnRotateCameraRightBegin(this);
			if (true == cInput.GetButton(kButtonRotateCameraRight))
				OnRotateCameraRightStay(this);
			if (true == cInput.GetButtonUp(kButtonRotateCameraRight))
				OnRotateCameraRightEnd(this);

			// kButtonFreeZoomCameraIn
			if (true == cInput.GetButtonDown(kButtonFreeZoomCameraIn))
				OnFreeZoomCameraInBegin(this);
			if (true == cInput.GetButton(kButtonFreeZoomCameraIn))
				OnFreeZoomCameraInStay(this);
			if (true == cInput.GetButtonUp(kButtonFreeZoomCameraIn))
				OnFreeZoomCameraInEnd(this);

			// kButtonFreeZoomCameraOut
			if (true == cInput.GetButtonDown(kButtonFreeZoomCameraOut))
				OnFreeZoomCameraOutBegin(this);
			if (true == cInput.GetButton(kButtonFreeZoomCameraOut))
				OnFreeZoomCameraOutStay(this);
			if (true == cInput.GetButtonUp(kButtonFreeZoomCameraOut))
				OnFreeZoomCameraOutEnd(this);

			// kButtonTiltCameraUp
			if (true == cInput.GetButtonDown(kButtonTiltCameraUp))
				OnTiltCameraUpBegin(this);
			if (true == cInput.GetButton(kButtonTiltCameraUp))
				OnTiltCameraUpStay(this);
			if (true == cInput.GetButtonUp(kButtonTiltCameraUp))
				OnTiltCameraUpEnd(this);

			// kButtonTiltCameraDown
			if (true == cInput.GetButtonDown(kButtonTiltCameraDown))
				OnTiltCameraDownBegin(this);
			if (true == cInput.GetButton(kButtonTiltCameraDown))
				OnTiltCameraDownStay(this);
			if (true == cInput.GetButtonUp(kButtonTiltCameraDown))
				OnTiltCameraDownEnd(this);

			// kButtonResetCameraToDefaults
			if (true == cInput.GetButtonDown(kButtonResetCameraToDefaults))
				OnResetCameraToDefaultsBegin(this);
			if (true == cInput.GetButton(kButtonResetCameraToDefaults))
				OnResetCameraToDefaultsStay(this);
			if (true == cInput.GetButtonUp(kButtonResetCameraToDefaults))
				OnResetCameraToDefaultsEnd(this);

			// kButtonZoomPreset0
			if (true == cInput.GetButtonDown(kButtonZoomPreset0))
				OnCameraZoomPreset0Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset0))
				OnCameraZoomPreset0Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset0))
				OnCameraZoomPreset0End(this);


			// kButtonZoomPreset1
			if (true == cInput.GetButtonDown(kButtonZoomPreset1))
				OnCameraZoomPreset1Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset1))
				OnCameraZoomPreset1Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset1))
				OnCameraZoomPreset1End(this);


			// kButtonZoomPreset2
			if (true == cInput.GetButtonDown(kButtonZoomPreset2))
				OnCameraZoomPreset2Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset2))
				OnCameraZoomPreset2Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset2))
				OnCameraZoomPreset2End(this);


			// kButtonZoomPreset3
			if (true == cInput.GetButtonDown(kButtonZoomPreset3))
				OnCameraZoomPreset3Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset3))
				OnCameraZoomPreset3Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset3))
				OnCameraZoomPreset3End(this);


			// kButtonZoomPreset4
			if (true == cInput.GetButtonDown(kButtonZoomPreset4))
				OnCameraZoomPreset4Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset4))
				OnCameraZoomPreset4Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset4))
				OnCameraZoomPreset4End(this);


			// kButtonZoomPreset5
			if (true == cInput.GetButtonDown(kButtonZoomPreset5))
				OnCameraZoomPreset5Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset5))
				OnCameraZoomPreset5Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset5))
				OnCameraZoomPreset5End(this);


			// kButtonZoomPreset6
			if (true == cInput.GetButtonDown(kButtonZoomPreset6))
				OnCameraZoomPreset6Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset6))
				OnCameraZoomPreset6Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset6))
				OnCameraZoomPreset6End(this);


			// kButtonZoomPreset7
			if (true == cInput.GetButtonDown(kButtonZoomPreset7))
				OnCameraZoomPreset7Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset7))
				OnCameraZoomPreset7Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset7))
				OnCameraZoomPreset7End(this);


			// kButtonZoomPreset8
			if (true == cInput.GetButtonDown(kButtonZoomPreset8))
				OnCameraZoomPreset8Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset8))
				OnCameraZoomPreset8Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset8))
				OnCameraZoomPreset8End(this);


			// kButtonZoomPreset9
			if (true == cInput.GetButtonDown(kButtonZoomPreset9))
				OnCameraZoomPreset9Begin(this);
			if (true == cInput.GetButton(kButtonZoomPreset9))
				OnCameraZoomPreset9Stay(this);
			if (true == cInput.GetButtonUp(kButtonZoomPreset9))
				OnCameraZoomPreset9End(this);


#if DEBUG
			if (cInput.GetKeyUp(kButtonDebugSpawnFriendlyShip))
			{
				//Debug.Log("Debug spawn friendly ship.");
				DynamicAgentDefinition.GetFromUniqueName(DynamicAgentDefinition.kDynamicAgentDefinitionStealthRecon).DebugSpawnFriendlyShip();
			}

			if (cInput.GetKeyUp(kButtonDebugSpawnHostileShip))
			{
				//Debug.Log("Debug spawn hostile ship.");
				DynamicAgentDefinition.GetFromUniqueName(DynamicAgentDefinition.kDynamicAgentDefinitionStealthRecon).DebugSpawnHostileShip();
			}
#endif
		}

		#endregion Main
		#region Events

		#endregion Events
	}
















































}