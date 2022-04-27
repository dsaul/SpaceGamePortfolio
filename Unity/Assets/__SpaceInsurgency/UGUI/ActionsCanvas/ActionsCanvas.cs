using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class ActionsCanvas : SharedCode.Behaviours.InstanceTracked<ActionsCanvas>
	{
		#region Variables

		[SerializeField]
		GameObject m_ItemPrefab;

		[Inspect]
		public GameObject ItemPrefab
		{
			get { return m_ItemPrefab; }
			set { m_ItemPrefab = value; }
		}

		[Inspect]
		public Transform itemRoot;

		Dictionary<string, GameObject> m_StaticItems;
		[Inspect]
		public Dictionary<string, GameObject> StaticItems
		{
			get {
				if (false == Application.isPlaying)
					return new Dictionary<string, GameObject>();
				else
					return m_StaticItems;
			}
		}

		Dictionary<Base, GameObject> m_DynamicItems;
		[Inspect]
		public Dictionary<Base, GameObject> DynamicItems
		{
			get
			{
				if (false == Application.isPlaying)
					return new Dictionary<Base, GameObject>();
				else
					return m_DynamicItems;
			}
		}

		[Inspect]
		public Sprite m_MoveSprite;
		[Inspect]
		public Sprite m_AttackSprite;
		[Inspect]
		public Sprite m_HoldSprite;
		[Inspect]
		public Sprite m_PatrolSprite;
		[Inspect]
		public Sprite m_StopSprite;

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			m_StaticItems = new Dictionary<string, GameObject>();
			m_DynamicItems = new Dictionary<Base, GameObject>();
		}

		protected override void Start()
		{
			base.Start();

			MakeHidden();
			SetupActionCanvas();

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

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;
			if (null == SelectionManager.Main)
				return;

			SelectionManager.Main.OnSelectionAdded += SelectionManager_OnSelectionAdded;
			SelectionManager.Main.OnSelectionRemoved += SelectionManager_OnSelectionRemoved;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == SelectionManager.Main)
				return;

			SelectionManager.Main.OnSelectionAdded -= SelectionManager_OnSelectionAdded;
			SelectionManager.Main.OnSelectionRemoved -= SelectionManager_OnSelectionRemoved;

			signalSub = false;
		}

		#endregion Setup
		#region Events

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
			//Debug.Log("SelectionManager_OnSelectionChanged");

			List<Base> keysToRemove = new List<Base>();
			keysToRemove.AddAll<Base>(m_DynamicItems.Keys);

			// Remove all dynamic.
			for (int i = 0; i < keysToRemove.Count; i++)
			{
				GameObject.Destroy(m_DynamicItems[keysToRemove[i]]);
				m_DynamicItems.Remove(keysToRemove[i]);
			}

			List<SpaceObject> selection = SelectionManager.Main.Selection;
			for (int i = 0; i < selection.Count; i++)
			{
				SpaceObject obj = selection[i];
				Assert.IsNotNull<SpaceObject>(obj);

				using (IEnumerator<Base> e = obj.ItemsShownInActionArea.GetEnumerator())
					while (e.MoveNext())
					{
						//Debug.Log("c.Current="+e.Current,e.Current);
						SelectionChangedAddItem(e.Current);
					}
			}
		}

		void SelectionChangedAddItem(Base item)
		{
			if (null == item)
				return;

			if (false == m_DynamicItems.ContainsKey(item))
				AddItem(Key: null, Item: item, Sprite: item.ActionSprite, OnClickAction: item.OnActionClick, CheckEnableFunc: item.IsConditionallyEnabled, DoTargetSelectFirst: item.DoTargetSelectFirst, IsStatic: false);
		}

		#endregion Events
		#region Main

		public void AddItem(string Key, Base Item, Sprite Sprite, Action OnClickAction, Func<bool> CheckEnableFunc, bool DoTargetSelectFirst = false, bool IsStatic = false)
		{
			GameObject obj = GameObject.Instantiate<GameObject>(m_ItemPrefab);
			obj.transform.SetParent(itemRoot, false);

			ActionsCanvasItem item = obj.GetComponent<ActionsCanvasItem>();
			item.OnClickAction = OnClickAction;
			item.CheckEnableFunc = CheckEnableFunc;
			item.DoTargetSelectFirst = DoTargetSelectFirst;
			item.IsStatic = IsStatic;
			item.CheckInteractableNow();



			Image img = obj.GetComponent<Image>();
			img.sprite = Sprite;

			if (true == IsStatic)
				m_StaticItems.Add(Key, obj);
			else
				m_DynamicItems.Add(Item, obj);

		}

		#endregion Main
		#region Public Functions

		public void MakeVisible()
		{
			GetComponent<Animator>().SetBool("visible", true);
		}

		public void MakeHidden()
		{
			GetComponent<Animator>().SetBool("visible", false);
		}

		#endregion Public Functions
		#region Setup Action Buttons

		void MoveOnClick()
		{
			if (null == SelectionManager.Main || false == SelectionManager.Main.enabled)
				return;
			
			List<SpaceObject> selection = SelectionManager.Main.Selection;

			for (int i = 0; i < selection.Count; i++)
			{
				SpaceObject spaceObject = selection[i];
				if (null == spaceObject.GetComponent<Engines>())
					continue;
				spaceObject.GetComponent<AI>().GetNeed<AINeedDestination>().SetWaypoint(InputManager.Main.MouseMovementPlaneLocationExact);
			}
		}

		bool MoveCheckEnable()
		{
			if (SelectionManager.Main.Selection.Count == 0)
				return false;


			if (false == SelectionManager.Main.SelectionHasEngines)
				return false;

			if (false == SelectionManager.Main.SelectionHasMovementSpeedAboveZero)
				return false;

			return true;
		}

		void AttackOnClick()
		{
			if (null == SelectionManager.Main || false == SelectionManager.Main.enabled)
				return;
			Debug.Log("Attack");

			List<SpaceObject> selection = SelectionManager.Main.Selection;

			if (0 == InputManager.Main.MouseOverSpaceObjectsFudged.Count) // attack move
			{
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					if (null == spaceObject.GetComponent<Engines>())
						continue;
					spaceObject.GetComponent<AI>().DoActionAttackMove(InputManager.Main.MouseMovementPlaneLocationExact);
				}
			}
			else
			{
				//Debug.Log("AA");
				SpaceObject closest = InputManager.Main.MouseOverSpaceObjectsFudged.TryGet(0);
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					if (null == spaceObject.GetComponent<Engines>())
						continue;

					AI ai = spaceObject.GetComponent<AI>();

					AINeedHostility ainh = ai.GetNeed<AINeedHostility>();
					if (null == ainh)
					{
						Debug.LogWarning("null == ainh");
						return;
					}

					ainh.LowestDispositionOverride = closest.GetComponent<DynamicAgent>();
				}
			}
		}

		bool AttackCheckEnabled()
		{
			if (SelectionManager.Main.Selection.Count == 0)
				return false;

			if (false == SelectionManager.Main.SelectionHasWeapons)
				return false;

			if (false == SelectionManager.Main.SelectionHasFireableWeapons)
				return false;

			return true;
		}

		void SetupActionCanvas()
		{
			ActionsCanvas.First.AddItem(Key: "ActionsMove", Item:null, Sprite: m_MoveSprite,
				OnClickAction: MoveOnClick,
				CheckEnableFunc: MoveCheckEnable, DoTargetSelectFirst: true, IsStatic: true);

			ActionsCanvas.First.AddItem(Key: "ActionsAttack", Item: null, Sprite: m_AttackSprite,
				OnClickAction: AttackOnClick,
				CheckEnableFunc: AttackCheckEnabled, DoTargetSelectFirst: true, IsStatic: true);

			ActionsCanvas.First.AddItem(Key: "ActionsHold", Item: null, Sprite: m_HoldSprite,
				OnClickAction: delegate {
					if (null == SelectionManager.Main || false == SelectionManager.Main.enabled)
						return;
				Debug.Log("Hold");

				List<SpaceObject> selection = SelectionManager.Main.Selection;
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					AI ai = spaceObject.GetComponent<AI>();
					if (null == ai)
						continue;
					ai.DoActionHoldPosition();
				}
			},
				CheckEnableFunc: delegate {
				if (SelectionManager.Main.Selection.Count == 0)
					return false;



				bool canHold = false;


				List<SpaceObject> selection = SelectionManager.Main.Selection;
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					AI ai = spaceObject.GetComponent<AI>();
					if (null == ai)
						continue;
					if (true == ai.CanDoActionHoldPosition)
					{
						canHold = true;
						break;
					}
				}

				return canHold;
			}, DoTargetSelectFirst: false, IsStatic: true);

			ActionsCanvas.First.AddItem(Key: "ActionsPatrol", Item: null, Sprite: m_PatrolSprite,
				OnClickAction: delegate {
					if (null == SelectionManager.Main || false == SelectionManager.Main.enabled)
						return;
				Debug.Log("Patrol");

				Vector3 patrolPoint = InputManager.Main.MouseMovementPlaneLocationExact;

				List<SpaceObject> selection = SelectionManager.Main.Selection;

				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					if (null == spaceObject.GetComponent<Engines>())
						continue;
					spaceObject.GetComponent<AI>().DoActionPatrol(patrolPoint);
				}
			},
				CheckEnableFunc: delegate {
				if (SelectionManager.Main.Selection.Count == 0)
					return false;

				bool canDoPatrol = false;


				List<SpaceObject> selection = SelectionManager.Main.Selection;
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					AI ai = spaceObject.GetComponent<AI>();
					if (null == ai)
						continue;
					if (true == ai.CanDoActionPatrol)
					{
						canDoPatrol = true;
						break;
					}
				}

				return canDoPatrol;
			}, DoTargetSelectFirst: true, IsStatic: true);

			ActionsCanvas.First.AddItem(Key: "ActionsStop", Item: null, Sprite: m_StopSprite,
				OnClickAction: delegate {
					if (null == SelectionManager.Main || false == SelectionManager.Main.enabled)
						return;
				Debug.Log("Stop");

				List<SpaceObject> selection = SelectionManager.Main.Selection;
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					AI ai = spaceObject.GetComponent<AI>();
					if (null == ai)
						continue;
					ai.DoActionStop();
				}
			},
				CheckEnableFunc: delegate {
				if (SelectionManager.Main.Selection.Count == 0)
					return false;


				bool canBeStopped = false;


				List<SpaceObject> selection = SelectionManager.Main.Selection;
				for (int i = 0; i < selection.Count; i++)
				{
					SpaceObject spaceObject = selection[i];
					AI ai = spaceObject.GetComponent<AI>();
					if (null == ai)
						continue;
					if (true == ai.CanDoActionStop)
					{
						canBeStopped = true;
						break;
					}
				}

				return canBeStopped;
			}, DoTargetSelectFirst: false, IsStatic: true);
		}




		#endregion Setup Action Buttons
	}
}