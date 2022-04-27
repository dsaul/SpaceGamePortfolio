using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using Vectrosity;
using SpaceInsurgency.Items;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class SelectionManager : SharedCode.Behaviours.InstanceTracked<SelectionManager>
	{
		#region Event Definitions

		public event Action<SelectionManager, SpaceObject> OnSelectionAdded; // SelectionManager sender, SpaceObject obj
		public event Action<SelectionManager, SpaceObject> OnSelectionRemoved; // SelectionManager sender, SpaceObject obj

		#endregion Event Definitions
		#region Variables

		List<SpaceObject> m_Selection;
		[Inspect]
		public List<SpaceObject> Selection
		{
			get {
				if (Application.isPlaying)
					return m_Selection;
				else
					return new List<SpaceObject>();
			}
		}

		


		bool m_MouseSelectActive = false;
		Vector3? m_MouseSelectDownPos;
		SpaceObject m_MouseSelectDownSpaceObjectClosest;

		bool m_CommandSelectionActive = false;
		List<VectorLine> m_CommandSelectionPlaceholderCircles = null;
		SpaceObject m_CommandSelectionClosestSpaceObject = null;
		Vector3? m_CommandSelectionMovementPlanePoint;
		VectorLine m_CommandSelectionFormationDirectionLine = null;
		GameObject m_CommandSelectPositionGameObject;



		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			OnSelectionAdded = new Action<SelectionManager, SpaceObject>(DefaultOnSelectionAdded);
			OnSelectionRemoved = new Action<SelectionManager, SpaceObject>(DefaultOnSelectionRemoved);
			m_Selection = new List<SpaceObject>();
			m_CommandSelectionPlaceholderCircles = new List<VectorLine>();
		}

		protected override void Start()
		{
			base.Start();

			SubscribeToEvents();

			m_CommandSelectPositionGameObject = new GameObject("Right Click Game Object");
			m_CommandSelectPositionGameObject.transform.parent = transform;

		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubscribeToEvents();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnsubscribeToEvents();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnsubscribeToEvents();
		}

		bool eventsSubscribed = false;
		void SubscribeToEvents()
		{
			if (true == eventsSubscribed)
				return;
			if (null == InputManager.Main)
				return;
			if (null == FTLManager.Main)
				return;

			InputManager.Main.OnPrimaryBegin += InputManager_OnPrimaryBegin;
			InputManager.Main.OnPrimaryStay += InputManager_OnPrimaryStay;
			InputManager.Main.OnPrimaryEnd += InputManager_OnPrimaryEnd;

			InputManager.Main.OnSecondaryBegin += InputManager_OnSecondaryBegin;
			InputManager.Main.OnSecondaryStay += InputManager_OnSecondaryStay;
			InputManager.Main.OnSecondaryEnd += InputManager_OnSecondaryEnd;

			FTLManager.Main.OnFTLTransitonBegin += FTLManager_OnFTLTransitonBegin;

			LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;

			eventsSubscribed = true;
		}
		
		void UnsubscribeToEvents()
		{
			if (false == eventsSubscribed)
				return;
			if (null == InputManager.Main)
				return;
			if (null == FTLManager.Main)
				return;

			InputManager.Main.OnPrimaryBegin -= InputManager_OnPrimaryBegin;
			InputManager.Main.OnPrimaryStay -= InputManager_OnPrimaryStay;
			InputManager.Main.OnPrimaryEnd -= InputManager_OnPrimaryEnd;

			InputManager.Main.OnSecondaryBegin -= InputManager_OnSecondaryBegin;
			InputManager.Main.OnSecondaryStay -= InputManager_OnSecondaryStay;
			InputManager.Main.OnSecondaryEnd -= InputManager_OnSecondaryEnd;

			FTLManager.Main.OnFTLTransitonBegin -= FTLManager_OnFTLTransitonBegin;

			LevelGeometry.OnLevelGeometryCreated -= LevelGeometry_OnLevelGeometryCreated;

			eventsSubscribed = false;
		}



		#endregion Setup
		#region Events

		void DefaultOnSelectionAdded(SelectionManager sender, SpaceObject obj)
		{
			if (0 == m_Selection.Count)
			{
#warning redo this for unity 5 (on select sounds)
			}

			// Add this to the selection.
			m_Selection.Add(obj);
		}

		void DefaultOnSelectionRemoved(SelectionManager sender, SpaceObject obj)
		{
			m_Selection.Remove(obj);

			// If we are no longer selecting the follow target we shouldn't follow it anymore.
			RtsCamera rtsCamera = CameraManager.Main.CameraGame.GetComponent<RtsCamera>();
			Transform followTarget = rtsCamera.FollowTarget;

			if (null != followTarget && false == followTarget.GetComponent<SpaceObject>().IsSelected)
				rtsCamera.EndFollow();
		}

		void InputManager_OnPrimaryBegin(InputManager sender)
		{
			MouseSelectButtonBegin(sender);
		}
		void InputManager_OnPrimaryStay(InputManager sender)
		{
			MouseSelectButtonStay(sender);
		}
		void InputManager_OnPrimaryEnd(InputManager sender)
		{
			MouseSelectButtonEnd(sender);
		}

		void InputManager_OnSecondaryBegin(InputManager sender)
		{
			CommandSelectionButtonBegin(sender);
		}
		void InputManager_OnSecondaryStay(InputManager sender)
		{
			CommandSelectionButtonStay(sender);
		}
		void InputManager_OnSecondaryEnd(InputManager sender)
		{
			CommandSelectionButtonEnd(sender);
		}

		void FTLManager_OnFTLTransitonBegin(FTLManager sender)
		{
			SelectionManager.Main.DeSelectAll();
		}

		void LevelGeometry_OnLevelGeometryCreated(LevelGeometry source, bool fromSaveFile)
		{
			DeSelectAll();

			Select(Faction.PlayerFaction.DynamicAgents);
		}

		#endregion Events
		#region Ancessors

		

		#endregion Ancessors
		#region Changing Selection

		public void Select(List<DynamicAgent> objs)
		{
			using (IEnumerator<DynamicAgent> e = objs.GetEnumerator())
				while (e.MoveNext())
					Select(e.Current.GetComponent<SpaceObject>());
		}

		public void Select(IEnumerable<SpaceObject> objs)
		{
			using (IEnumerator<SpaceObject> e = objs.GetEnumerator())
				while (e.MoveNext())
					Select(e.Current);
		}

		public void Select(SpaceObject obj)
		{
			if (null == obj)
				return;
			if (m_Selection.Contains(obj))
				return;

			if (true == FTLManager.Main.TransitionActive)
				return;

			// We only select planets, ships, and space stations.
			if (null == obj.GetComponent<DynamicAgent>() && null == obj.GetComponent<StaticAgent>())
				return;

			// Some things disallow selection.
			if (null != obj.GetComponent<StaticAgent>() && true == obj.GetComponent<StaticAgent>().DisallowSelection)
				return;

			// We don't select things that are [in the proccess of being] destroyed.
			if (null != obj.GetComponent<Health>() && true == obj.GetComponent<Health>().IsDestroyed)
				return;
			
			OnSelectionAdded(this, obj);
		}

		public void DeSelect(SpaceObject obj)
		{
			OnSelectionRemoved(this, obj);
		}

		public void DeSelectAll(SpaceObject exception = null)
		{
			int i = m_Selection.Count;
			while (--i >= 0)
				if (m_Selection[i] != exception)
					OnSelectionRemoved(this, m_Selection[i]);
		}

		public bool IsSelected(SpaceObject selObj)
		{
			return m_Selection.Contains(selObj);
		}



		#endregion Changing Selection
		#region Mouse Select



		void MouseSelectButtonBegin(InputManager obj)
		{
			if (InputManager.Main.CurrentInputMode != InputManager.InputMode.Game)
				return;

			// If we're in the galaxy map, mouse select is disabled.
			if (true == LevelGeometry.Main.IsGalaxyMap)
				return;

			m_MouseSelectDownPos = Input.mousePosition;
			m_MouseSelectDownSpaceObjectClosest = InputManager.Main.MouseOverSpaceObjectsFudged.TryGet(0);

			m_MouseSelectActive = true;
		}

		void MouseSelectButtonStay(InputManager obj)
		{
			if (false == m_MouseSelectActive)
				return;

			// If the mouse strays this far away from the down point at any time we're going to cancel the click.

			Vector3 mousePosition = Input.mousePosition;

			// Allow the user to cancel their click by moving the mouse away from the click point and letting go.
			if (// Mouse is not within 30 px of mouse down position.
				false == mousePosition.x.IsBetween(m_MouseSelectDownPos.Value.x - 10f, m_MouseSelectDownPos.Value.x + 10f) ||
				false == mousePosition.y.IsBetween(m_MouseSelectDownPos.Value.y - 10f, m_MouseSelectDownPos.Value.y + 10f))
			{
				m_MouseSelectActive = false;
				return;
			}
		}

		void MouseSelectButtonEnd(InputManager obj)
		{
			if (false == m_MouseSelectActive)
				return;

			Vector3 mousePosition = Input.mousePosition;
			
			do
			{
				if (// If we mouse up over a UI Element it is not a mouse select event.
				true == InputManager.Main.IsOverUIElement ||

				// Mouse is not within 30 px of mouse down position.
				false == mousePosition.x.IsBetween(m_MouseSelectDownPos.Value.x - 10f, m_MouseSelectDownPos.Value.x + 10f) ||
				false == mousePosition.y.IsBetween(m_MouseSelectDownPos.Value.y - 10f, m_MouseSelectDownPos.Value.y + 10f))
					break;

				MouseSelectImpl(m_MouseSelectDownSpaceObjectClosest);
			}
			while (false);

			m_MouseSelectDownPos = null;
			m_MouseSelectActive = false;
		}

		public void MouseSelectImpl(SpaceObject Object = null)
		{
			// If nothing was clicked on deselect everything.
			if (null == Object)
			{
				SelectionManager.Main.DeSelectAll();
				return;
			}

			if (true == InputManager.Main.IsLeftDoubleClickFrame)
			{
				//Debug.Log("Double click " + mouseSelectDownSpaceObjectClosest);

				// Shouldn't be necessary, but just to be sure.
				Select(Object);

				CameraManager.Main.CameraGame.GetComponent<RtsCamera>().Follow(Object.transform);
			}
			else // false == source.IsDoubleClickFrame
			{
				//Debug.Log("Single click " + mouseSelectDownSpaceObjectClosest);

				if (true == InputManager.Main.ModifierKeyActive)
				{
					if (Object.IsSelected)
						DeSelect(Object);
					else
						Select(Object);
				}
				else
				{
					DeSelectAll(Object);
					Select(Object);
				}
			}
		}

		#endregion Mouse Select
		#region Command Selection

		void CommandSelectionButtonBegin(InputManager source)
		{
			if (source.CurrentInputMode != InputManager.InputMode.Game)
				return;

			// If we have no selection, don't bother.
			if (Selection.Count == 0)
				return;


			m_CommandSelectionMovementPlanePoint = InputManager.Main.MouseMovementPlaneLocationExact;
			m_CommandSelectionClosestSpaceObject = InputManager.Main.MouseOverSpaceObjectsExact.TryGet<SpaceObject>(0);

			for (int i=0; i<m_Selection.Count; i++)
			{
				VectorLine vl = new VectorLine("CommandSelectionLine", new Vector3[30], null, 1f, LineType.Continuous);
				vl.MakeCircle(m_CommandSelectionMovementPlanePoint.Value, Vector3.up, .5f);
				vl.color = Const.Color.WhiteA50;
				vl.Draw3DAuto();
				m_CommandSelectionPlaceholderCircles.Add(vl);
			}

			if (null == m_CommandSelectionFormationDirectionLine)
			{
				m_CommandSelectionFormationDirectionLine = new VectorLine("Formation Direction Line", new Vector3[2] { m_CommandSelectionMovementPlanePoint.Value, m_CommandSelectionMovementPlanePoint.Value }, null, 1f, LineType.Continuous);
				m_CommandSelectionFormationDirectionLine.color = Const.Color.WhiteA50;
				m_CommandSelectionFormationDirectionLine.Draw3DAuto();
			}

			m_CommandSelectPositionGameObject.transform.position = m_CommandSelectionMovementPlanePoint.Value;
			m_CommandSelectPositionGameObject.transform.localRotation = Quaternion.identity;

			m_CommandSelectionActive = true;
		}

		void CommandSelectionButtonStay(InputManager source)
		{
			if (false == m_CommandSelectionActive)
				return;

			// Get the current mouse location as we want to orient the destination points to where the player is dragging to.
			Vector3 movementPlanePoint = InputManager.Main.MouseMovementPlaneLocationExact;

			// Draw a line from mouse down position to mouse stay position.
			m_CommandSelectionFormationDirectionLine.points3[1] = movementPlanePoint;

			// Make the right click object point toward the mouse stay position so that the transform will correctly adapt the local to world points.
			m_CommandSelectPositionGameObject.transform.LookAt(movementPlanePoint);

			// Get the cached formation for the current selection.
			List<Vector3> points = FormationSquareish.Get(Const.FormationSpacing, m_Selection.Count);

#warning replace this with either hologram or wireframe representations of the ships.
			// Draw circles.
			for (int i=0; i<points.Count; i++)
				m_CommandSelectionPlaceholderCircles[i].MakeCircle(m_CommandSelectPositionGameObject.transform.TransformPoint(points[i]), Vector3.up, .5f);
		}

		void CommandSelectionButtonEnd(InputManager source)
		{
			if (false == m_CommandSelectionActive)
				return;
			
			do
			{
				bool goToRightClickedUnit = false;
				
				// On Unit
				if (null != m_CommandSelectionClosestSpaceObject)
				{
					//Debug.Log("On Unit");

					Faction.Relation mouseOverRelation = m_CommandSelectionClosestSpaceObject.GetComponent<Membership>().Faction.DispositionOfPlayer;

					// If this is in the galaxy map we want to move always.
					if (null != m_CommandSelectionClosestSpaceObject.GetComponent<GalaxyMapDestination>())
						mouseOverRelation = Faction.Relation.Neutral;

					switch (mouseOverRelation)
					{
						case Faction.Relation.Friendly:
						case Faction.Relation.Amicable:
						case Faction.Relation.Neutral:
							goToRightClickedUnit = true;

#warning TODO: Have this point be a bit away from the target ship so that it doesn't push the original ship out of the way.
							m_CommandSelectionMovementPlanePoint = m_CommandSelectionClosestSpaceObject.transform.position;
							m_CommandSelectPositionGameObject.transform.position = m_CommandSelectionClosestSpaceObject.transform.position;
							break;
						case Faction.Relation.Hostile:
						case Faction.Relation.Cautious:
#warning TODO: Test hostile actions here.
							List<SpaceObject> selection = SelectionManager.Main.Selection;
							for (int i = 0; i < selection.Count; i++)
							{
								AINeedHostility ainh = selection[i].GetComponent<AI>().GetNeed<AINeedHostility>();
								if (null != ainh)
									ainh.LowestDispositionOverride = m_CommandSelectionClosestSpaceObject.GetComponent<DynamicAgent>();
							}
							break;

					}



					
				}
				
				// Movement
				if (null == m_CommandSelectionClosestSpaceObject || true == goToRightClickedUnit)
				{
					if (null == m_CommandSelectionMovementPlanePoint)
						break;
					
					//Debug.Log("Just movement");

					List<Vector3> points = FormationSquareish.Get(Const.FormationSpacing, m_Selection.Count);
					
					bool playWaypointSetAnim = false;

					for (int i = 0; i < m_Selection.Count; i++)
					{
						SpaceObject spaceObject = m_Selection[i];
						if (null == spaceObject.GetComponent<Engines>())
							continue;

						Vector3 pt = m_CommandSelectPositionGameObject.transform.TransformPoint(points[i]);

						if (null != spaceObject.GetComponent<AI>())
						{
							AINeedDestination aind = spaceObject.GetComponent<AI>().GetNeed<AINeedDestination>();
							if (null != aind)
							{
								aind.SetWaypoint(pt, source.ModifierKeyActive);

								AINeedHostility ainh = spaceObject.GetComponent<AI>().GetNeed<AINeedHostility>();
								if (null != ainh)
									ainh.LowestDispositionOverride = null;
							}
						}
						playWaypointSetAnim = true;
					}

					if (true == playWaypointSetAnim)
						RTS3DArrow.MakeNewArrowAtPosition(m_CommandSelectionMovementPlanePoint.Value);
				}
				



				
			}
			while (false);

			VectorLine.Destroy(ref m_CommandSelectionFormationDirectionLine);
			m_CommandSelectionFormationDirectionLine = null;

			int d = m_CommandSelectionPlaceholderCircles.Count;
			while (--d >= 0)
			{
				VectorLine vl = m_CommandSelectionPlaceholderCircles[d];
				VectorLine.Destroy(ref vl);
				m_CommandSelectionPlaceholderCircles.RemoveAt(d);
			}
			m_CommandSelectionActive = false;
			m_CommandSelectionMovementPlanePoint = null;
		}

		#endregion Command Selection
		#region Anccessors

		[Inspect, Group("Runtime")]
		public bool SelectionHasEngines
		{
			get
			{
				if (false == Application.isPlaying)
					return false;

				for (int i = 0; i < m_Selection.Count; i++)
					if (null != m_Selection[i].GetComponent<Engines>())
						return true;
				
				return false;
			}
		}

		[Inspect, Group("Runtime")]
		public bool SelectionHasMovementSpeedAboveZero
		{
			get
			{
				if (false == Application.isPlaying)
					return false;

				for (int i = 0; i < m_Selection.Count; i++)
				{
					Engines e = m_Selection[i].GetComponent<Engines>();

					if (null == e)
						continue;

					if (e.MovementSpeed > 0)
						return true;
				}

				return false;
			}
		}

		[Inspect, Group("Runtime")]
		public bool SelectionHasWeapons
		{
			get
			{
				if (false == Application.isPlaying)
					return false;

				for (int i = 0; i < m_Selection.Count; i++)
					if (null != m_Selection[i].GetComponent<Weapons>())
						return true;

				return false;
			}
		}

		[Inspect, Group("Runtime")]
		public bool SelectionHasFireableWeapons
		{
			get
			{
				if (false == Application.isPlaying)
					return false;

				for (int i = 0; i < m_Selection.Count; i++)
				{
					Weapons w = m_Selection[i].GetComponent<Weapons>();

					if (null == w)
						continue;

					if (true == w.CanDoAttackType(Weapons.AttackType.Orbit) || w.CanDoAttackType(Weapons.AttackType.Strafing))
						return true;
				}

				return false;
			}
		}

		#endregion Ancessors
		#region Utility


		#endregion Utility
	}
}
