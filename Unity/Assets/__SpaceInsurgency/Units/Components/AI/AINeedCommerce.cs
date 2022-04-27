using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;

namespace SpaceInsurgency
{
	public class AINeedCommerce : AINeed2
	{
		public override int Importance // higher the more important
		{
			get
			{
				return 25;
			}
		}

		public override int Urgency
		{
			get
			{
				return 25;
			}
		}

		public StaticAgent chosenPlanetDestination = null;
		public Vector3? pointAroundChosenPlanet = null;

		public Vector3? departPoint = null;

		protected override void Update()
		{
			base.Update();

			// Choose a planet to go to.
			if (null == chosenPlanetDestination)
			{
				List<StaticAgent> tradePlanets = new List<StaticAgent>();
				
				for (int i=0; i<StaticAgent.Instances.Count; i++)
					if (true == StaticAgent.Instances[i].IsTradeDestination)
						tradePlanets.Add(StaticAgent.Instances[i]);

				if (tradePlanets.Count > 0)
					chosenPlanetDestination = tradePlanets[UnityEngine.Random.Range(0, tradePlanets.Count)];
			}

			// Get point around planet.
			if (null != chosenPlanetDestination && null == pointAroundChosenPlanet)
			{
				pointAroundChosenPlanet = chosenPlanetDestination.GetNewIdleAndSpawnPoint();
			}

			if (null == departPoint)
			{
				departPoint = chosenPlanetDestination.GetNewDepartPoint(50);
			}
		}

		bool arrivedAtChosenPlanet = false;
		float? departTime = null;
		bool hasDepartedFromChosenPlanet = false;
		bool hasCalledFTLOut = false;

		public override void PriorityLottery()
		{
			base.PriorityLottery();

			if (null == pointAroundChosenPlanet)
				return;
			if (null == pointAroundChosenPlanet)
				return;
			if (false == ai.CanSetWaypoints)
				return;

			if (false == arrivedAtChosenPlanet && pointAroundChosenPlanet.Value.SqrDistance(ai.transform.position) < (4f*4f))
			{
				arrivedAtChosenPlanet = true;
				departTime = Time.time + UnityEngine.Random.Range(15, 30);
			}

			if (false == arrivedAtChosenPlanet)
			{
				ai.SetWaypoints(this, new List<Vector3>() { pointAroundChosenPlanet.Value });
				return;
			}

			if (false == hasDepartedFromChosenPlanet && true == arrivedAtChosenPlanet && Time.time < departTime.Value)
				return;

			if (false == hasDepartedFromChosenPlanet && true == arrivedAtChosenPlanet && Time.time > departTime.Value)
			{
				ai.SetWaypoints(this, new List<Vector3>() { departPoint.Value });
				hasDepartedFromChosenPlanet = true;
				return;
			}

			if (false == hasCalledFTLOut && true == hasDepartedFromChosenPlanet && departPoint.Value.SqrDistance(ai.transform.position) < (4f * 4f))
			{
				ai.GetComponent<FTL>().DoFTLOut();
				hasCalledFTLOut = true;
				return;
			}
		}
	}
}
