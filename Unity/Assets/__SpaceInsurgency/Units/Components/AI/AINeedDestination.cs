using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	/// <summary>
	/// A simple provider that tells the ship to go to its destination if it has one.
	/// </summary>
	[AdvancedInspector]
	public class AINeedDestination : AINeed2
	{
		#region Variables

		/// <summary>
		/// We maintain our own copy of the waypoints as the ones stored in the engines 
		/// script may be replaced by other AINeedsProvider instances if required.
		/// </summary>
		List<Vector3> m_Waypoints;

		[Inspect]
		public List<Vector3> Waypoints
		{
			get { return m_Waypoints; }
			set { m_Waypoints = value; }
		}

		#endregion Variables
		#region Anccessors

		[Inspect]
		public override int Importance
		{
			get
			{
				return 5;
			}
		}

		[Inspect]
		public override int Urgency
		{
			get
			{
				if (m_Waypoints.Count == 0)
					return 0;
				return 5;
			}
		}

		#endregion Anccessors
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			m_Waypoints = new List<Vector3>();
		}

		#endregion Setup


		/// <summary>
		/// Just to make sure we don't have duplicate delegate subscribers.
		/// </summary>
		bool subscribedToEngines = false;

		public override void PriorityLottery()
		{
			base.PriorityLottery();

			// If this is true we are the most important wrt moving.
			if (true == ai.CanSetWaypoints)
			{
				// We only set waypoints this way if we are taking control from something 
				// else. If we do this for everything we get stuttering movement.
				if (this != ai.waypointsSetBy)
					ai.SetWaypoints(this, m_Waypoints);

				
				if (false == subscribedToEngines)
				{
					// Setup event listeners.
					ai.GetComponent<Engines>().OnHasArrivedAtWaypoint += engines_OnHasArrivedAtWaypoint;
					ai.GetComponent<Engines>().OnHasArrivedAtFinalWaypoint += engines_OnHasArrivedAtWaypoint;
					subscribedToEngines = true;
				}
				
			}
		}

		void engines_OnHasArrivedAtWaypoint(Engines source, Vector3 point)
		{
			if (this == ai.waypointsSetBy)
			{
				if (m_Waypoints[0].SqrDistance(point) < 0.5f) // fucking floating point errors.
					m_Waypoints.RemoveAt(0);
			}
			else
			{
				Debug.Log("something else has control");
				if (true == subscribedToEngines)
				{
					ai.GetComponent<Engines>().OnHasArrivedAtWaypoint -= engines_OnHasArrivedAtWaypoint;
					ai.GetComponent<Engines>().OnHasArrivedAtFinalWaypoint -= engines_OnHasArrivedAtWaypoint;
					subscribedToEngines = false;
				}
				
			}
			
		}

		
		/// <summary>
		/// The publically visable destination setter.
		/// </summary>
		/// <param name="point">Destination waypoint.</param>
		/// <param name="addWaypointInstead">Does this replace all the old waypoints, or somewhere to go after?</param>
		public void SetWaypoint(Vector3 point, bool addWaypointInstead = false)
		{
			//Debug.Log("SetWaypoint");

			if (true == addWaypointInstead)
			{
				m_Waypoints.Add(point);
				if (this == ai.waypointsSetBy)
					ai.AddWaypoint(this, point);
			}
			else
			{
				m_Waypoints.Clear();
				m_Waypoints.Add(point);
				ai.SetWaypoints(this, m_Waypoints);
			}
		}


	}
}

