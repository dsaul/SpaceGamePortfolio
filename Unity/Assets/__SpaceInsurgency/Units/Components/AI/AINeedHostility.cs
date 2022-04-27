//#define DEBUG_ORBIT_ONLY_OVERRIDE
//#define DEBUG_STRAF_ONLY_OVERRIDE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class AINeedHostility : AINeed2
	{
		DynamicAgent m_LowestDispositionOverride = null;
		[Inspect]
		public DynamicAgent LowestDispositionOverride
		{
			get { return m_LowestDispositionOverride; }
			set { m_LowestDispositionOverride = value; }
		}

		#region Types

		public enum Stance
		{
			Agressive = 1,
			DefendIfAttacked = 2,
			Passive = 3,
		};

		#endregion Types
		#region Disposition

		public Dictionary<DynamicAgent, DispositionInfo> dispositions = new Dictionary<DynamicAgent, DispositionInfo>();
		public List<DispositionInfo> lowestDispositions = new List<DispositionInfo>();
		public int lowestDispositionNum = int.MaxValue;

		

		void RecalculateLowestDisposition()
		{
			UpdateDispositions();
			
			// Seeing enemies makes the ai slightly antsy.

			lowestDispositions.Clear();
			lowestDispositionNum = int.MaxValue;

			// If we have an override for lowest disposition... ie. player chose to attack someone specific. Don't have anything else in that array.
			if (null != m_LowestDispositionOverride)
			{
				lowestDispositions.Add(dispositions[m_LowestDispositionOverride]);

				lowestDispositionNum = -50;

				return;
			}

			// Find the most hated by this ship.
			using (IEnumerator<KeyValuePair<DynamicAgent, DispositionInfo>> e = dispositions.GetEnumerator())
				while (e.MoveNext())
				{
					KeyValuePair<DynamicAgent, DispositionInfo> cur = e.Current;
					if (cur.Value.Disposition > lowestDispositionNum)
						continue;
					else if (cur.Value.Disposition == lowestDispositionNum)
						lowestDispositions.Add(cur.Value);
					else if (cur.Value.Disposition < lowestDispositionNum)
					{
						lowestDispositions.Clear();
						lowestDispositionNum = cur.Value.Disposition;
						lowestDispositions.Add(cur.Value);
					}
				}


		}

		void UpdateDispositions()
		{
			// Remove expired ships.
			List<DynamicAgent> d = new List<DynamicAgent>();
			using (IEnumerator<KeyValuePair<DynamicAgent, DispositionInfo>> e = dispositions.GetEnumerator())
				while (e.MoveNext())
					if (null == e.Current.Key)
						d.Add(e.Current.Key);

			for (int i = 0; i < d.Count; i++)
				dispositions.Remove(d[i]);

			// Go through and remove any expired ships.
			int j = lowestDispositions.Count;
			while (--j >= 0)
				if (null == lowestDispositions[j].ship)
					lowestDispositions.RemoveAt(j);

			// Search for new ships.
			List<DynamicAgent> ships = DynamicAgent.Instances;
			for (int i = 0; i < ships.Count; i++)
			{
				DynamicAgent ship = ships[i];

				// if we already have this ship in our aggro dictionary skip it.
				if (true == dispositions.ContainsKey(ship))
					continue;

				// Don't add itself.
				if (ai.GetComponent<DynamicAgent>() == ship)
					continue;

				// If we're warping in, don't calculate anything yet.
				if (true == ai.GetComponent<FTL>().FTLActive)
					continue;

				// If the other ship is warping in, don't target it yet.
				if (true == ship.GetComponent<FTL>().FTLActive)
					continue;

#warning add skip if not in sensor range

				DispositionInfo info = new DispositionInfo();
				info.ship = ship;
				info.initialOpinion = ai.GetComponent<Membership>().Faction.GetFactionDispositionOn(ship.GetComponent<Membership>().Faction);

				dispositions.Add(ship, info);
			}
		}

		#endregion Disposition
		#region Attack


		public Weapons.AttackType chosenAttackType;
		public float chosenAttackTypeUntilTime = float.MinValue;

		public override void PriorityLottery()
		{
			base.PriorityLottery();

			if (0 == lowestDispositions.Count)
				RecalculateLowestDisposition();
			if (0 == lowestDispositions.Count)
				return;

			// Get closest lowest, also check if we're hostile.
			DynamicAgent otherShip = null;
			if (null == m_LowestDispositionOverride)
			{
				float sqrDistance = float.MaxValue;
				for (int i = 0; i < lowestDispositions.Count; i++)
				{
					if (lowestDispositions[i].Disposition > -5)
						continue;

					DynamicAgent aShip = lowestDispositions[i].ship;
					if (null == aShip)
						continue;

					float thisSqrDis = ai.transform.position.SqrDistance(aShip.transform.position);
					if (thisSqrDis < sqrDistance)
					{
						sqrDistance = thisSqrDis;
						otherShip = aShip;
					}
				}
			}
			else
			{
				otherShip = m_LowestDispositionOverride;
			}
			

			// If there are no ships to attack don't do anything.
			if (null == otherShip)
				return;

			ai.GetComponent<Sensors>().Target = otherShip.GetComponent<SpaceObject>();


			bool canDoSrafing = ai.GetComponent<Weapons>().CanDoAttackType(Weapons.AttackType.Strafing);
			bool canDoOrbit = ai.GetComponent<Weapons>().CanDoAttackType(Weapons.AttackType.Orbit);
			if (false == canDoSrafing && false == ai.GetComponent<Weapons>())
				return;

			

			if (false == ai.CanSetWaypoints)
				return;

			// We are allowed to move, this will be towards the enemy if not within range. Or do whatever attacking motion is required.
			if (Time.time > chosenAttackTypeUntilTime)
			{
				//Debug.Log("Choose new attack type.");
				
				float chosenTimeDuration = UnityEngine.Random.Range(15, 45);

#if DEBUG_ORBIT_ONLY_OVERRIDE
				chosenAttackType = Weapons.AttackType.Orbit;
				chosenAttackTypeUntilTime = Time.time + chosenTimeDuration;
#elif DEBUG_STRAF_ONLY_OVERRIDE
				chosenAttackType = Weapons.AttackType.Strafing;
				chosenAttackTypeUntilTime = Time.time + chosenTimeDuration;
#else
				if (true == canDoSrafing && false == canDoOrbit)
				{
					chosenAttackType = Weapons.AttackType.Strafing;
					chosenAttackTypeUntilTime = Time.time + chosenTimeDuration;
				}
				else if (false == canDoSrafing && true == canDoOrbit)
				{
					chosenAttackType = Weapons.AttackType.Orbit;
					chosenAttackTypeUntilTime = Time.time + chosenTimeDuration;
				}
				else
				{
					bool coinFlip = UnityEngine.Random.Range(0, 2) == 1;
					if (true == coinFlip)
					{
						chosenAttackType = Weapons.AttackType.Strafing;
						chosenAttackTypeUntilTime = Time.time + chosenTimeDuration;
					}
					else
					{
						chosenAttackType = Weapons.AttackType.Orbit;
						chosenAttackTypeUntilTime = Time.time + chosenTimeDuration;
					}
				}
#endif
			}

			switch (chosenAttackType)
			{
				case Weapons.AttackType.Orbit:
					DoMovementOrbit();
					break;
				case Weapons.AttackType.Strafing:
					DoMovementStrafing();
					break;
			}

			ai.GetComponent<Weapons>().CheckWeaponsFire(chosenAttackType);
			
		}

		float? chosenDistance;

		void DoMovementOrbit()
		{
			Weapons aiWeapons = ai.GetComponent<Weapons>();
			
			//Debug.Log( "Attack!" + ai.sensors.Target, ai);
			float maxRange = aiWeapons.OrbitMaxRange;
			float maxRangeSqr = maxRange * maxRange;
			float minRange = aiWeapons.OrbitMinRange;
			float minRangeSqr = minRange * minRange;

			float sqrDistanceToTarget = ai.transform.position.SqrDistance(ai.GetComponent<Sensors>().Target.transform.position);

			// We need to have a chosen distance for orbiting.
			// If we are already beetween minRange and maxRange we'll choose whatever we currently are.
			// Otherwise we choose randomly between the two.
			if (null == chosenDistance)
			{
				if (sqrDistanceToTarget.IsBetween<float>(minRangeSqr, maxRangeSqr))
				{
					chosenDistance = Mathf.Sqrt(sqrDistanceToTarget);
				}
				else
				{
					chosenDistance = UnityEngine.Random.Range(minRange, maxRange);
				}
			} 

			// this may have to change based upon range from target
			//float marginOfErrorSqr = 2 * 2;
			float marginOfErrorSqr = 4 * 4;

			//Debug.Log("sqrDistanceToTarget " + sqrDistanceToTarget + " maxRangeSqr " + maxRangeSqr + " minRangeSqr " + minRangeSqr);

			Transform otherTransform = ai.GetComponent<Sensors>().Target.transform;
			Vector3 aiPos = ai.transform.position;
			Vector3 aiPosOtherLocal = otherTransform.InverseTransformPoint(aiPos);
			Vector3 aiPosOtherLocalNormalized = aiPosOtherLocal.normalized;
			Vector3 aiPosOtherLocalNormalizedDistance = aiPosOtherLocalNormalized * chosenDistance.Value;

			// If we aren't within 2sqr of the target then head towards them.
			if (false == sqrDistanceToTarget.IsBetween<float>((chosenDistance.Value * chosenDistance.Value) - marginOfErrorSqr, (chosenDistance.Value * chosenDistance.Value) + marginOfErrorSqr))
			{
				//Debug.Log("Move Towards");
				ai.SetWaypoints(this, new List<Vector3>() { otherTransform.TransformPoint(aiPosOtherLocalNormalizedDistance) });
			}
			else
			{
				//Debug.Log("Orbit");
				Vector3 rotatedPoint = aiPosOtherLocalNormalizedDistance.RotateAroundPivot(Vector3.zero, Quaternion.Euler(new Vector3(0, 5, 0)));

				ai.SetWaypoints(this, new List<Vector3>() { otherTransform.TransformPoint(rotatedPoint) });
			}
		}

		bool movingAway = false;
		Vector3? m_StrafeMovingAwayPointAroundTarget;

		void DoMovementStrafing()
		{
			if (true == movingAway)
			{
				if (null != m_StrafeMovingAwayPointAroundTarget)
				{
					// we may have to reset the waypoint here

					if (ai.transform.position.SqrDistance(m_StrafeMovingAwayPointAroundTarget.Value) < 0.1f * 0.1f)
					{
						movingAway = false;
						m_StrafeMovingAwayPointAroundTarget = null;
					}

				}
			}
			else
			{
				if (ai.transform.position.SqrDistance(ai.GetComponent<Sensors>().Target.transform.position) > 4f * 4f)
				{
					ai.SetWaypoints(this, new List<Vector3>() { GetStrafeRunPointCloseToTarget() });
				}
				else
				{
					m_StrafeMovingAwayPointAroundTarget = GetStrafeFarAwayPoint();
					ai.SetWaypoints(this, new List<Vector3>() { m_StrafeMovingAwayPointAroundTarget.Value });
					movingAway = true;
				}
			}
		}



		Vector3 GetStrafeRunPointCloseToTarget()
		{
			Vector3 targetPosWorld = ai.GetComponent<Sensors>().Target.transform.position;
			Vector3 targetPosLocal = ai.transform.InverseTransformPoint(targetPosWorld);
			Vector3 targetDirectionLocal = targetPosLocal.normalized;
			Vector3 targetDirectionWorld = ai.transform.TransformDirection(targetDirectionLocal);
			Vector3 destination = targetPosWorld - (targetDirectionWorld * 2);

			NavMeshHit navmeshPositionSample;
			if (true == NavMesh.SamplePosition(destination, out navmeshPositionSample, 50, 1 << NavMesh.GetAreaFromName("Walkable")))
				return navmeshPositionSample.position;
			else
				Debug.LogWarning("Could not get NavMeshHit for input vector " + destination);

			//Debug.Log(destination);
			return destination;
		}

		Vector3 GetStrafeFarAwayPoint()
		{
			Vector3 localBehind = Vector3.forward.RotateAroundPivot(Vector3.zero, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(90, 270), 0)));

			float random = UnityEngine.Random.Range(15, 25);
			Vector3 localMultiplied = localBehind * random;
			Vector3 worldBehind = ai.transform.TransformPoint(localMultiplied);
			//Debug.Log("point "+worldBehind);

			NavMeshHit navmeshPositionSample;
			if (true == NavMesh.SamplePosition(worldBehind, out navmeshPositionSample, 50, 1 << NavMesh.GetAreaFromName("Walkable")))
				return navmeshPositionSample.position;
			else
				Debug.LogWarning("Could not get NavMeshHit for input vector " + worldBehind);

			return worldBehind;
		}


		#endregion Attack
		#region Events

		protected override void Update()
		{
			base.Update();

			UpdateDispositions();
		}

		#endregion Events
		#region Properties

		public override int Importance // higher the more important
		{
			get { return 80; }
		}

		public override int Urgency
		{
			get
			{
				RecalculateLowestDisposition();

				if (lowestDispositionNum < 0)
					return -lowestDispositionNum.Clamp(-40, 0);

				return 0;
			}
		}

		#endregion Properties

	}
}
