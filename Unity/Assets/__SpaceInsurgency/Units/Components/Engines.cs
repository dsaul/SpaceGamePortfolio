using AdvancedInspector;
using SharedCode;
using SpaceInsurgency.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Vectrosity;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Engines : EquippableComponent
	{
		#region Event Definitions

		public Action<Engines, List<Vector3>> _OnDestinationCleared; // Engines source, List<Vector3> formerWaypoints
		public Action<Engines, List<Vector3>> OnDestinationCleared
		{
			get
			{
				if (null == _OnDestinationCleared)
					_OnDestinationCleared = new Action<Engines, List<Vector3>>(delegate {
						m_Waypoints.Clear();
					});
				return _OnDestinationCleared;
			}
			set { _OnDestinationCleared = value;  }
		}

		public event Action<Engines, List<Vector3>> OnHasDeparted; // Engines source, List<Vector3> currentWaypoints
		public event Action<Engines, Vector3> OnHasArrivedAtFinalWaypoint; // Engines source, Vector3 point
		public event Action<Engines, Vector3> OnHasArrivedAtWaypoint; // Engines source, Vector3 point
		public event Action<Engines, bool, bool, bool, bool> OnMovement; // Engines source, bool isMoving, bool startedMoving, bool stillMoving, bool endingMoving
		public event Action<Engines, bool, bool, bool, bool> OnRotateLeft; // Engines source, bool isRotatingLeft, bool startedRotatingLeft, bool stillRotatingLeft, bool endingRotatingLeft
		public event Action<Engines, bool, bool, bool, bool> OnRotateRight; // Engines source, bool isRotatingRight, bool startedRotatingRight, bool stillRotatingRight, bool endingRotatingRight

		#endregion Event Definitions
		#region Variables

		bool m_DebugLines = false;
		[Inspect]
		public bool DebugLines
		{
			get { return m_DebugLines; }
			set { m_DebugLines = value; }
		}

		public List<Vector3> m_Waypoints = new List<Vector3>();

		#endregion Variables
		#region Setup

		

		protected override void Awake()
		{
			base.Awake();

			
			OnHasDeparted = new Action<Engines, List<Vector3>>(delegate { 
				//Debug.Log("OnHasDeparted");
			});
			OnHasArrivedAtFinalWaypoint = new Action<Engines, Vector3>(delegate(Engines source, Vector3 point) {
				//Debug.Log("OnHasArrivedAtFinalWaypoint");
			});
			OnHasArrivedAtWaypoint = new Action<Engines, Vector3>(delegate { });
			OnMovement = new Action<Engines, bool, bool, bool, bool>(delegate(Engines source, bool isMoving, bool startedMoving, bool stillMoving, bool endingMoving) { });
			OnRotateLeft = new Action<Engines, bool, bool, bool, bool>(delegate(Engines source, bool isRotatingLeft, bool startedRotatingLeft, bool stillRotatingLeft, bool endingRotatingLeft) { });
			OnRotateRight = new Action<Engines, bool, bool, bool, bool>(delegate(Engines source, bool isRotatingRight, bool startedRotatingRight, bool stillRotatingRight, bool endingRotatingRight) { });
			OnFinishedSpawnSetup += delegate {
				PushMS();

				//Debug.Log("OnFinishedSpawnSetup " + ship.parts);

				if (null != GetComponent<DynamicAgent>() && null != GetComponent<DynamicAgent>().Parts)
				{
					GetComponent<NavMeshAgent>().radius = GetComponent<DynamicAgent>().Parts.navMeshAgentRadius;
					//Debug.Log("set radius " + radius);
				}
					
			};

			OnMovement += OnMovementCheckIdleStartMoving;
			OnMovement += OnMovementCheckHasDeparted;
			OnMovement += OnMovementCheckAtWaypoint;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (null == GetComponent<NavMeshAgent>())
			{
				NavMeshAgent agent = gameObject.AddComponent<NavMeshAgent>();

				//Debug.Log("OnEnable");
				if (null != GetComponent<DynamicAgent>() && null != GetComponent<DynamicAgent>().Parts)
				{
					GetComponent<NavMeshAgent>().radius = GetComponent<DynamicAgent>().Parts.navMeshAgentRadius;
					//Debug.Log("set radius on enable "+radius);
				}

				agent.height = 2f;
				agent.baseOffset = 0f;
				agent.speed = MovementSpeed;
				agent.angularSpeed = 300f;
				agent.acceleration = 50f;
				agent.stoppingDistance = 0.1f;
				agent.autoBraking = true;
				agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
				agent.avoidancePriority = 50;
				agent.autoTraverseOffMeshLink = true;
				agent.autoRepath = true;
			}

			//navMeshAgent.enabled = true;
			
			GetComponent<Health>().OnShipDestroyed += OnShipDestroyed;
		}

		protected override void OnDisable()
		{
			base.OnEnable();

			//navMeshAgent.enabled = false;

			GameObject.Destroy(GetComponent<NavMeshAgent>());

			if (null != GetComponent<Health>())
				GetComponent<Health>().OnShipDestroyed -= OnShipDestroyed;
		}

		void PushMS()
		{
			cachedMovementSpeed = null;
			GetComponent<NavMeshAgent>().speed = MovementSpeed;
		}

		#endregion Setup
		#region Main

		// Position changing
		Vector3? lastPosition = null;
		bool wasMoving = false;


		// Rotation changing
		Quaternion? lastRotation = null;
		bool wasRotatingLeft = false;
		bool wasRotatingRight = false;

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			// If the ship has moved since the last update, then enable the rear thrusters, if not don't.
			Vector3 curPos = transform.position;
			if (false == lastPosition.HasValue)
				lastPosition = curPos;

			bool isMoving = lastPosition != curPos;
			bool startedMoving = false == wasMoving && true == isMoving;
			bool stillMoving = true == wasMoving && true == isMoving;
			bool endingMoving = true == wasMoving && false == isMoving;

			OnMovement(this, isMoving, startedMoving, stillMoving, endingMoving);

			wasMoving = isMoving;
			lastPosition = curPos;

			// If the ship has rotated since the last update, then enable the turning thrusters.
			Quaternion curRot = transform.rotation;
			if (false == lastRotation.HasValue)
				lastRotation = curRot;

			// I don't actually understand the next bit, copied it from:
			// http://answers.unity3d.com/questions/26783/how-to-get-the-signed-angle-between-two-quaternion.html
			// get a "forward vector" for each rotation.
			Vector3 lastForward = lastRotation.Value * Vector3.forward;
			Vector3 curForward = curRot * Vector3.forward;

			// get a numeric angle for each vector, on the X-Z plane (relative to world forward)
			float angleA = Mathf.Atan2(lastForward.x, lastForward.z) * Mathf.Rad2Deg;
			float angleB = Mathf.Atan2(curForward.x, curForward.z) * Mathf.Rad2Deg;

			// get the signed difference in these angles
			float angleDiff = Mathf.DeltaAngle(angleA, angleB);

			// We make sure that we only consider it rotating if the absolute angle 
			// difference is greater then one, otherwise a thruster will always be on.
			bool isRotatingLeft = Mathf.Abs(angleDiff) > 1 && angleDiff < 0;
			bool startedRotatingLeft = false == wasRotatingLeft && true == isRotatingLeft;
			bool stillRotatingLeft = true == wasRotatingLeft && true == isRotatingLeft;
			bool endingRotatingLeft = true == wasRotatingLeft && false == isRotatingLeft;

			OnRotateLeft(this, isRotatingLeft, startedRotatingLeft, stillRotatingLeft, endingRotatingLeft);
			wasRotatingLeft = isRotatingLeft;

			bool isRotatingRight = Mathf.Abs(angleDiff) > 1 && angleDiff > 0;
			bool startedRotatingRight = false == wasRotatingRight && true == isRotatingRight;
			bool stillRotatingRight = true == wasRotatingRight && true == isRotatingRight;
			bool endingRotatingRight = true == wasRotatingRight && false == isRotatingRight;

			OnRotateRight(this, isRotatingRight, startedRotatingRight, stillRotatingRight, endingRotatingRight);
			wasRotatingRight = isRotatingRight;

			lastRotation = curRot;
		}


		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 interpretedVelocityPerFrame;

		Vector3? lastPos;
		protected override void Update()
		{
			base.Update();

			// Interpolate the velocity.
			if (null == lastPos)
				lastPos = transform.position;
			interpretedVelocityPerFrame = (transform.position - lastPos.Value) / Time.deltaTime;
			lastPos = transform.position;
			
			

			UpdateWaypointVisualDisplay();
		}

		#endregion Main
		#region Events

		void OnShipDestroyed(Health source)
		{
			enabled = false;
			
			ClearWaypoints();
		}

		void OnMovementCheckIdleStartMoving(Engines source, bool isMoving, bool startedMoving, bool stillMoving, bool endingMoving)
		{
			if (false == GetComponent<NavMeshAgent>().enabled)
				return;
			if (true == isMoving)
				return;
			if (true == startedMoving)
				return;
			if (true == stillMoving)
				return;
			if (true == endingMoving)
				return;
			if (m_Waypoints.Count == 0)
				return;

			GetComponent<NavMeshAgent>().destination = m_Waypoints[0];
		}

		void OnMovementCheckHasDeparted(Engines source, bool isMoving, bool startedMoving, bool stillMoving, bool endingMoving)
		{
			// If we just started moving with a destination (as opposed to being 
			// pushed around by something else) announce we have departed.
			if (startedMoving && m_Waypoints.Count != 0)
				OnHasDeparted(this, m_Waypoints);
		}

		void OnMovementCheckAtWaypoint(Engines source, bool isMoving, bool startedMoving, bool stillMoving, bool endingMoving)
		{
			if (m_Waypoints.Count == 1 && true == AtFinalWaypoint)
			{
				OnHasArrivedAtFinalWaypoint(this, m_Waypoints[0]);
				m_Waypoints.RemoveAt(0);
			}
			else if (m_Waypoints.Count > 1 && true == AtCurrentWaypoint)
			{
				OnHasArrivedAtWaypoint(this, m_Waypoints[0]);
				m_Waypoints.RemoveAt(0);

				if (m_Waypoints.Count != 0)
					GetComponent<NavMeshAgent>().destination = m_Waypoints[0];
			}
		}

		#endregion Events
		#region Pathfinding

		

		public void AddWaypoint(Vector3 destPoint)
		{
			//Debug.Log("AddWaypoint " + destPoint);

			// Can't add a waypoint to something that is being destroyed.
			if (true == GetComponent<Health>().IsDestroyed)
				return;

			NavMeshHit navmeshPositionSample;
			if (true == NavMesh.SamplePosition(destPoint, out navmeshPositionSample, 50, 1 << NavMesh.GetAreaFromName("Walkable")))
				m_Waypoints.Add(navmeshPositionSample.position);
			else
				Debug.LogWarning("Could not get NavMeshHit for input vector " + destPoint);
		}

		public void ClearAndSetWaypoint(Vector3 destPoint)
		{
			bool hadWaypoints = m_Waypoints.Count != 0;

			ClearWaypoints();
			
			AddWaypoint(destPoint);

			if (m_Waypoints.Count != 0)
				GetComponent<NavMeshAgent>().destination = m_Waypoints[0];

			// If we had waypoints we were moving already, we only want to call this 
			// if we were already moving. Otherwise this delegate is handled by update.
			if (true == hadWaypoints)
				OnHasDeparted(this, m_Waypoints);
		}

		public void ClearAndSetWaypoints(IEnumerable<Vector3> container)
		{
			bool hadWaypoints = m_Waypoints.Count != 0;

			ClearWaypoints();
			
			using (IEnumerator<Vector3> e = container.GetEnumerator())
					while (e.MoveNext())
						AddWaypoint(e.Current);

			if (m_Waypoints.Count != 0 && null != GetComponent<NavMeshAgent>())
				GetComponent<NavMeshAgent>().destination = m_Waypoints[0];

			// If we had waypoints we were moving already, we only want to call this 
			// if we were already moving. Otherwise this delegate is handled by update.
			if (true == hadWaypoints)
				OnHasDeparted(this, m_Waypoints);
		}

		public void ClearWaypoints()
		{
			OnDestinationCleared(this, m_Waypoints);
		}

		VectorLine movementLine;

		void UpdateWaypointVisualDisplay()
		{
			if (false == m_DebugLines)
				return;

			if (null != movementLine && m_Waypoints.Count == 0)
			{
				VectorLine.Destroy(ref movementLine);
				movementLine = null;
			}
			if (m_Waypoints.Count == 0)
				return;
			
			int lineSize = m_Waypoints.Count + 1;

			if (null != movementLine && movementLine.points3.Count != lineSize)
				movementLine.Resize(lineSize);

			if (null == movementLine)
			{
				Vector3[] arr = new Vector3[lineSize];
				arr[0] = transform.position;

				for (int i = 1; i < lineSize; i++)
					arr[i] = m_Waypoints[i - 1];

				movementLine = new VectorLine("Movement Line", arr, null, 1f, LineType.Continuous, Joins.Fill);
				movementLine.color = new Color(0f, 1f, 0f, 0.2f);
				movementLine.Draw3DAuto();
			}
			else
			{
				movementLine.points3[0] = transform.position;

				for (int i = 1; i < m_Waypoints.Count + 1; i++)
					movementLine.points3[i] = m_Waypoints[i - 1];

				//vl.Draw3D();
			}
		}





















		float atWaypointLeeway = 0.15f;
		
		public bool AtCurrentWaypoint
		{
			get
			{
				if (m_Waypoints.Count == 0)
					return true;

				return transform.position.SqrDistance(m_Waypoints[0]) < (atWaypointLeeway * atWaypointLeeway);
			}
		}

		public bool AtFinalWaypoint
		{
			get
			{
				if (m_Waypoints.Count == 0)
					return true;
				return transform.position.SqrDistance(m_Waypoints[m_Waypoints.Count - 1]) < (atWaypointLeeway * atWaypointLeeway);
			}
		}

		#endregion Pathfinding
		#region Movement / Warp

		float? cachedMovementSpeed = null;

		[Inspect, Group("Runtime")]
		public float MovementSpeed
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;

				if (false == cachedMovementSpeed.HasValue)
				{
					float s = 0f;
					for (int i = 0; i < Items.Count; i++)
						s += Items[i].ItemDefinition.MaxSpeedModifier;
					for (int i = 0; i < GlobalItemsManager.Main.GlobalItems.Count; i++)
						s += GlobalItemsManager.Main.GlobalItems[i].MaxSpeedModifier;

					DynamicAgent da = GetComponent<DynamicAgent>();
					Assert.IsNotNull<DynamicAgent>(da);
					
					DynamicAgentDefinition dad = da.AgentDefinition;
					Assert.IsNotNull<DynamicAgentDefinition>(dad);

					cachedMovementSpeed = dad.MoveSpeedBase + s;
				}

				return cachedMovementSpeed.GetValueOrDefault(0f);
			}
		}

		public bool ObjectsBlockingFTLPath
		{
			get
			{
				Vector3 fwd = transform.TransformDirection(Vector3.forward);
				if (Physics.Raycast(transform.position, fwd, Mathf.Infinity))
					return true;
				else
					return false;
			}
		}

		public Vector3 FTLWarpOutEndPosition
		{
			get
			{
				return transform.TransformPoint(Vector3.forward * 500);
			}
		}

		#endregion Movement / Warp
		#region Anccessor

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;

			return newItem.CanBeAddedToEngines ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
		}

		#endregion Anccessor
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();
			public List<Vector3> waypoints = new List<Vector3>();

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);
			serialized.waypoints.AddAll<Vector3>(m_Waypoints);

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			for (int i = 0; i < serialized.items.Count; i++)
				AddItem(Base.GetFromUniqueName(serialized.items[i]));
			ClearAndSetWaypoints(serialized.waypoints);
		}

		#endregion Serialization
	}

































}