using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;

namespace SpaceInsurgency
{
	public class AINeedPolice : AINeed2
	{
		public override int Importance // higher the more important
		{
			get
			{
				return 50;
			}
		}

		public override int Urgency
		{
			get
			{
				return 10;
			}
		}

		public StaticAgent chosenPlanetDestination = null;
		public Vector3? pointAroundChosenPlanet = null;

		protected override void Update()
		{
			base.Update();


			// Choose a planet to go to.
			if (null == chosenPlanetDestination)
			{
				chosenPlanetDestination = StaticAgent.Random;
			}

			// Get point around planet.
			if (null != chosenPlanetDestination && null == pointAroundChosenPlanet)
			{
				pointAroundChosenPlanet = chosenPlanetDestination.GetNewIdleAndSpawnPoint();
			}
		}

		public override void PriorityLottery()
		{
			base.PriorityLottery();

			if (null == pointAroundChosenPlanet)
				return;
			if (null == pointAroundChosenPlanet)
				return;
			if (false == ai.CanSetWaypoints)
				return;

			if (pointAroundChosenPlanet.Value.SqrDistance(ai.transform.position) < (4f*4f))
			{
				chosenPlanetDestination = null;
				pointAroundChosenPlanet = null;
			}
			else
			{
				ai.SetWaypoints(this, new List<Vector3>() { pointAroundChosenPlanet.Value });
			}
		}
	}
}
