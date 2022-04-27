using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class AI : SharedCode.Behaviours.Base
	{
		#region Variables
		

		int m_AILoopIndexCurrent = int.MinValue;
		/// <summary>
		/// Every time the ai loops through it increments this. Certain actions such as movement 
		/// are claimed by the highest priority needs. Since we are keeping a count we get to 
		/// determine if we can claim this action this loop. When it reaches int.MaxValue, it 
		/// is looped back to int.MinValue + 1.
		/// </summary>
		[Inspect, Group("Base")]
		public int AILoopIndexCurrent
		{
			get { return m_AILoopIndexCurrent; }
		}


		GameObject m_AINeedsRoot;
		[Inspect, Group("Base")]
		public GameObject AINeedsRoot
		{
			get
			{
				if (null == m_AINeedsRoot)
				{
					m_AINeedsRoot = new GameObject("AI Needs Root");
					m_AINeedsRoot.transform.parent = transform;
					m_AINeedsRoot.transform.localPosition = Vector3.zero;
					m_AINeedsRoot.transform.localRotation = Quaternion.identity;
					m_AINeedsRoot.transform.localScale = Vector3.one;
				}
				
				return m_AINeedsRoot; 
			}
			set { m_AINeedsRoot = value; }
		}

		Dictionary<string, AINeed2> m_Needs;
		/// <summary>
		/// The current needs.
		/// </summary>
		[Inspect]
		public Dictionary<string, AINeed2> Needs
		{
			get
			{
				if (null == m_Needs)
					m_Needs = new Dictionary<string, AINeed2>();
				return m_Needs;
			}
		}

		List<AINeed2> m_SortedNeeds;
		[Inspect]
		public List<AINeed2> SortedNeeds
		{
			get 
			{
				if (null == m_SortedNeeds)
					m_SortedNeeds = new List<AINeed2>();
				return m_SortedNeeds; 
			}
			set { m_SortedNeeds = value; }
		}
		

		#endregion Variables
		#region Setup

		

		#endregion Setup
		#region Events

		protected override void Update()
		{
			base.Update();
			
			m_AILoopIndexCurrent++;

			// Loop value if required.
			if (m_AILoopIndexCurrent == int.MaxValue)
				m_AILoopIndexCurrent = int.MinValue + 1;
			
			// Sort the needs by urgency in decending order.
			m_SortedNeeds.Sort(delegate(AINeed2 a, AINeed2 b) { return -a.CompareTo(b); });

			// Go through the needs.
			for (int i=0; i<Needs.Count; i++)
				m_SortedNeeds[i].PriorityLottery();
		}

		#endregion Events
		#region Anccessors

		public T GetNeed<T>() where T : AINeed2
		{
			AINeed2 ret = null;
			m_Needs.TryGetValue(typeof(T).FullName, out ret);
			return ret as T;
		}

		#endregion Anccessors
		#region Mutators

		public T AddNeed<T>(T type) where T : AINeed2
		{
			return AddNeed(type) as T;
		}

		public AINeed2 AddNeed(Type type)
		{
			Assert.IsNotNull<Type>(type);

			GameObject needObj = new GameObject(type.FullName);
			needObj.transform.parent = AINeedsRoot.transform;
			needObj.transform.localPosition = Vector3.zero;
			needObj.transform.localRotation = Quaternion.identity;
			needObj.transform.localScale = Vector3.one;

			AINeed2 instance = needObj.AddComponent(type) as AINeed2;
			Assert.IsNotNull<AINeed2>(instance);

			instance.ai = this;

			Needs[type.FullName] = instance;
			SortedNeeds.Add(instance);

			return instance;
		}

		public void RemoveNeed(AINeed2 oldNeed)
		{
			oldNeed.ai = null;

			m_Needs.Remove(oldNeed.GetType().FullName);
			m_SortedNeeds.Remove(oldNeed);

			GameObject.Destroy(oldNeed.gameObject);
		}

		

		#endregion Mutators
		#region Waypoints

		public AINeed2 waypointsSetBy = null;
		int waypointsLoopIndex = int.MinValue;
		public bool CanSetWaypoints
		{
			get { return waypointsLoopIndex != AILoopIndexCurrent; }
		}
		public void SetWaypoints(AINeed2 sender, List<Vector3> list)
		{
			if (false == CanSetWaypoints)
				return;

			if (null != GetComponent<Engines>())
				GetComponent<Engines>().ClearAndSetWaypoints(list);

			waypointsLoopIndex = AILoopIndexCurrent;
			waypointsSetBy = sender;
		}
		public void AddWaypoint(AINeed2 sender, Vector3 point)
		{
			if (sender != waypointsSetBy)
				throw new Exception("take ownership with SetWaypoints before calling AddWaypoint");

			if (null != GetComponent<Engines>())
				GetComponent<Engines>().AddWaypoint(point);

			waypointsLoopIndex = AILoopIndexCurrent;
		}

		#endregion Waypoints
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> needs = new List<string>();
			
			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			using (IEnumerator<string> e = m_Needs.Keys.GetEnumerator())
				while (e.MoveNext())
					serialized.needs.Add(e.Current);

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			for (int i=0; i<serialized.needs.Count; i++)
			{
				string classname = serialized.needs[i];
				Type type = Type.GetType(classname);
				Assert.IsNotNull<Type>(type);

				
				AddNeed(type);

			}
		}

		#endregion Serialization































#warning write ai system do it stack based with priorities



		public bool CanDoActionStop
		{
			get
			{
#warning complete
#if false
				if (null != overseerShipPatrolPlanetsRandomly && true == overseerShipPatrolPlanetsRandomly.enabled)
					return true;
				if (null != actionMove && true == actionMove.enabled)
					return true;
				if (null != actionAttackTarget && true == actionAttackTarget.enabled)
					return true;
				if (null != actionPatrol && true == actionPatrol.enabled)
					return true;
#endif
				return false;
			}
		}

		[ContextMenu("DoActionStop()")]
		public void DoActionStop()
		{
#if false
			if (null != overseerShipPatrolPlanetsRandomly)
				overseerShipPatrolPlanetsRandomly.enabled = false;
			if (null != actionMove)
				actionMove.enabled = false;
			if (null != actionAttackTarget)
				actionAttackTarget.enabled = false;
			if (null != actionPatrol)
				actionPatrol.enabled = false;
			sensors.Target = null;

			engines.ClearWaypoints();
#endif
		}

		

		public void DoActionAttackMove(Vector3 point)
		{
#if false
			if (null != overseerShipPatrolPlanetsRandomly)
				overseerShipPatrolPlanetsRandomly.enabled = false;
			if (null != actionHoldPosition)
				actionHoldPosition.enabled = false;
			if (null != actionMove)
				actionMove.enabled = false;
			if (null != actionAttackTarget)
				actionAttackTarget.enabled = false;
			if (null != actionPatrol)
				actionPatrol.enabled = false;
			sensors.Target = null;

			engines.ClearAndSetWaypoint(point);

			if (null == actionAttackMove)
				gameObject.AddComponent<ActionAttackMove>();
			actionAttackMove.enabled = true;
#endif
		}

		public bool CanDoActionHoldPosition
		{
			get
			{
				return false;
			}
		}

		[ContextMenu("DoActionHoldPosition()")]
		public void DoActionHoldPosition()
		{
#if false
			if (null != overseerShipPatrolPlanetsRandomly)
				overseerShipPatrolPlanetsRandomly.enabled = false;
			if (null != actionMove)
				actionMove.enabled = false;
			if (null != actionAttackMove)
				actionAttackMove.enabled = false;
			if (null != actionAttackTarget)
				actionAttackTarget.enabled = false;
			if (null != actionPatrol)
				actionPatrol.enabled = false;
			sensors.Target = null;

			engines.ClearWaypoints();

			if (null == actionHoldPosition)
				gameObject.AddComponent<ActionHoldPosition>();
			actionHoldPosition.enabled = true;
#endif
		}

		

		public bool CanDoActionPatrol
		{
			get
			{
				return false;
			}
		}

		public void DoActionPatrol(Vector3 point)
		{
#if false
			if (null != overseerShipPatrolPlanetsRandomly)
				overseerShipPatrolPlanetsRandomly.enabled = false;
			if (null != actionMove)
				actionMove.enabled = false;
			if (null != actionAttackMove)
				actionAttackMove.enabled = false;
			if (null != actionHoldPosition)
				actionHoldPosition.enabled = false;
			if (null != actionAttackTarget)
				actionAttackTarget.enabled = false;
			sensors.Target = null;

			engines.ClearWaypoints();

			if (null == actionPatrol)
				gameObject.AddComponent<ActionPatrol>();
			actionPatrol.Waypoint1 = point;
			actionPatrol.Waypoint2 = transform.position;
			actionPatrol.enabled = true;
#endif
		}
	}
}