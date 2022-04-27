using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInsurgency.Mouseover
{
	public class MouseoverOverlayCanvas : SharedCode.Behaviours.InstanceTracked<MouseoverOverlayCanvas>
	{
		#region Variables (Selection Indicators)

		List<SpaceObject> m_SelectionIndicatorsToRemoveTmp;

		Dictionary<SpaceObject, GameObject> m_SelectionIndicators;

		[Inspect, ReadOnly, Group("Runtime (Selection Indicators)")]
		public Dictionary<SpaceObject, GameObject> SelectionIndicators
		{
			get { return m_SelectionIndicators; }
			set { m_SelectionIndicators = value; }
		}

		[SerializeField]
		GameObject m_SelectionIndicatorPrefab;

		[Inspect, Group("Editor (Selection Indicators)"), DontAllowSceneObject]
		public GameObject SelectionIndicatorPrefab
		{
			get { return m_SelectionIndicatorPrefab; }
			set { m_SelectionIndicatorPrefab = value; }
		}

		[SerializeField]
		Transform m_SelectionIndicatorInstanceParent;

		[Inspect, Group("Editor (Selection Indicators)")]
		public Transform SelectionIndicatorInstanceParent
		{
			get { return m_SelectionIndicatorInstanceParent; }
			set { m_SelectionIndicatorInstanceParent = value; }
		}

		#endregion Variables (Selection Indicators)
		#region Variables (Statistic Bars)

		List<SpaceObject> m_StatisticBarsToRemoveTmp;
		Dictionary<SpaceObject, GameObject> m_StatisticBars;

		[Inspect, ReadOnly, Group("Runtime (Statistic Bars)")]
		public Dictionary<SpaceObject, GameObject> StatisticBars
		{
			get { return m_StatisticBars; }
			set { m_StatisticBars = value; }
		}

		[SerializeField]
		GameObject m_StatisticBarPrefab;

		[Inspect, Group("Editor (Statistic Bars)"), DontAllowSceneObject]
		public GameObject StatisticBarPrefab
		{
			get { return m_StatisticBarPrefab; }
			set { m_StatisticBarPrefab = value; }
		}

		[SerializeField]
		Transform m_StatisticBarInstanceParent;

		[Inspect, Group("Editor (Statistic Bars)")]
		public Transform StatisticBarInstanceParent
		{
			get { return m_StatisticBarInstanceParent; }
			set { m_StatisticBarInstanceParent = value; }
		}

		#endregion Variables (Statistic Bars)
		#region Variables (Detail Text)

		[SerializeField]
		DetailText m_DetailText;

		[Inspect, Group("Editor (Detail Text)")]
		public DetailText DetailText
		{
			get { return m_DetailText; }
			set { m_DetailText = value; }
		}
		#endregion Variables (Detail Text)
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			m_SelectionIndicators = new Dictionary<SpaceObject, GameObject>();
			m_SelectionIndicatorsToRemoveTmp = new List<SpaceObject>();

			m_StatisticBars = new Dictionary<SpaceObject, GameObject>();
			m_StatisticBarsToRemoveTmp = new List<SpaceObject>();
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
			if (null == SelectionManager.Main)
				return;
			if (null == InputManager.Main)
				return;

			SelectionManager.Main.OnSelectionAdded += SelectionManager_OnSelectionAdded;
			SelectionManager.Main.OnSelectionRemoved += SelectionManager_OnSelectionRemoved;
			InputManager.Main.OnMouseoverSpaceObjectFudgedChanged += InputManager_OnMouseoverSpaceObjectFudgedChanged;

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
			InputManager.Main.OnMouseoverSpaceObjectFudgedChanged -= InputManager_OnMouseoverSpaceObjectFudgedChanged;

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
			UpdateSelectionIndicators();
			UpdateStatisticBars();
		}

		void InputManager_OnMouseoverSpaceObjectFudgedChanged(InputManager obj, List<SpaceObject> objects)
		{
			UpdateStatisticBars();
		}

		#endregion Events
		#region Selection Indicators

		
		void UpdateSelectionIndicators()
		{
			List<SpaceObject> selection = SelectionManager.Main.Selection;


			// Remove indicators for those who aren't selected anymore.
			m_SelectionIndicatorsToRemoveTmp.Clear();

			using (IEnumerator<SpaceObject> enumerator = m_SelectionIndicators.Keys.GetEnumerator())
				while (enumerator.MoveNext())
					if (false == selection.Contains(enumerator.Current))
						m_SelectionIndicatorsToRemoveTmp.Add(enumerator.Current);


			int i = m_SelectionIndicatorsToRemoveTmp.Count;
			while (--i >= 0)
			{
				GameObject.Destroy(m_SelectionIndicators[m_SelectionIndicatorsToRemoveTmp[i]]);
				m_SelectionIndicators.Remove(m_SelectionIndicatorsToRemoveTmp[i]);
				m_SelectionIndicatorsToRemoveTmp.RemoveAt(i);
			}


			for (i = 0; i < selection.Count; i++)
			{
				SpaceObject selected = selection[i];
				if (m_SelectionIndicators.ContainsKey(selected))
					continue;

				GameObject obj = GameObject.Instantiate<GameObject>(m_SelectionIndicatorPrefab);
				obj.transform.SetParent(m_SelectionIndicatorInstanceParent, false);
				obj.GetComponent<SelectionIndicatorGroup>().Target = selected;
				m_SelectionIndicators.Add(selected, obj);
			}

			// Order the transforms.
			foreach (KeyValuePair<SpaceObject, GameObject> item in m_SelectionIndicators.OrderBy(key => key.Key.SqrDistanceToGameCamera))
				item.Value.transform.SetAsFirstSibling();
		}

		#endregion Selection Indicators
		#region Statistic Bars

		void UpdateStatisticBars()
		{
			List<SpaceObject> selection = SelectionManager.Main.Selection;
			SpaceObject mouseOver = InputManager.Main.MouseOverSpaceObjectsFudged.TryGet<SpaceObject>(0);

			// Remove status bars that no longer apply.
			m_StatisticBarsToRemoveTmp.Clear();

			using (IEnumerator<SpaceObject> enumerator = m_StatisticBars.Keys.GetEnumerator())
				while (enumerator.MoveNext())
					if (false == selection.Contains(enumerator.Current) && enumerator.Current != mouseOver)
						m_StatisticBarsToRemoveTmp.Add(enumerator.Current);

			int i = m_StatisticBarsToRemoveTmp.Count;
			while (--i >= 0)
			{
				DestroyStatBar(m_StatisticBarsToRemoveTmp[i]);
				m_StatisticBarsToRemoveTmp.RemoveAt(i);
			}

			// If we're in the galaxy map we don't show any stat bars.
			if (true == LevelGeometry.Main.IsGalaxyMap)
				return;


			// Add selected ships that don't have statistic bars yet.
			for (i = 0; i < selection.Count; i++)
				InstantiateStatBar(selection[i]);

			// Add the ship the mouse is currently over.
			if (null != mouseOver)
				InstantiateStatBar(mouseOver);

			// Order the transforms.
			foreach (KeyValuePair<SpaceObject, GameObject> item in m_StatisticBars.OrderBy(key => key.Key.SqrDistanceToGameCamera))
				item.Value.transform.SetAsFirstSibling();
		}

		void InstantiateStatBar(SpaceObject spaceObject)
		{
			if (null == spaceObject)
				return;
			if (m_StatisticBars.ContainsKey(spaceObject))
				return;

			GameObject obj = GameObject.Instantiate<GameObject>(m_StatisticBarPrefab);
			obj.transform.SetParent(m_StatisticBarInstanceParent, false);
			obj.GetComponent<StatisticBars>().Target = spaceObject;
			m_StatisticBars.Add(spaceObject, obj);

			Health health = spaceObject.GetComponent<Health>();
			if (health != null)
				health.OnShipDestroyed += StatBar_OnShipDestroyed;
		}

		void DestroyStatBar(SpaceObject spaceObject)
		{
			if (null == spaceObject)
				return;

			Health health = spaceObject.GetComponent<Health>();
			if (health != null)
				health.OnShipDestroyed -= StatBar_OnShipDestroyed;

			GameObject toDestroy;
			if (m_StatisticBars.TryGetValue(spaceObject, out toDestroy))
				GameObject.Destroy(toDestroy);
				
			m_StatisticBars.Remove(spaceObject);
		}

		void StatBar_OnShipDestroyed(Health sender)
		{
			DestroyStatBar(sender.GetComponent<SpaceObject>());
		}

		#endregion Statistic Bars
		






	}
}

