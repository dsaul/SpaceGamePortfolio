using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public class SpawnerImperialPatrol : SharedCode.Behaviours.Base
	{
		public int targetShipCount = 2;


		float? startTime;
		protected override void Start()
		{
			base.Start();

			startTime = Time.time;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetTimer(1f, true, Main_OnTimeTickOneSecond);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			ClearTimer(Main_OnTimeTickOneSecond);
		}

		float delayUntil = float.MinValue;

		void Main_OnTimeTickOneSecond()
		{
			if (Time.time < delayUntil)
				return;

			int factionShipCount = 0;

			List<DynamicAgent> factionAgents = Faction.ImperialFaction.DynamicAgents;
			for (int i = 0; i < factionAgents.Count; i++)
			{
				if (null == factionAgents[i].GetComponent<AI>().GetNeed<AINeedPolice>())
					continue;
				factionShipCount++;
			}

			if (factionShipCount >= targetShipCount)
			{
				// So that if the ships are destroyed there is a little delay before reinforcements show up.
				delayUntil = Time.time + 30f;
				return;
			}


			for (int i = factionShipCount; i < targetShipCount; i++)
			{
				Vector3 spawnPoint = StaticAgent.Random.GetNewIdleAndSpawnPoint();

				NavMeshHit hit;
				if (true == NavMesh.SamplePosition(spawnPoint, out hit, 50, 1 << NavMesh.GetAreaFromName("Walkable")))
					spawnPoint = hit.position;
				else
					Debug.LogWarning("Could not get NavMeshHit for input vector " + spawnPoint);

				DynamicAgent.Serialized serialized = new DynamicAgent.Serialized();
				serialized.dynamicAgentDefinition = DynamicAgentDefinition.kDynamicAgentDefinitionInterceptor;
				serialized.position = spawnPoint;
				serialized.rotation = Quaternion.identity;

				serialized.membership.faction = Faction.kFactionImperial;

				serialized.weapons.items.Add(Base.kWeaponLightBlaster);

				serialized.ai.needs.Add(typeof(AINeedDestination).FullName);
				serialized.ai.needs.Add(typeof(AINeedHostility).FullName);
				serialized.ai.needs.Add(typeof(AINeedPolice).FullName);

				DynamicAgentDefinition.Spawn(serialized);

				DynamicAgent ship = DynamicAgentDefinition.Spawn(serialized);
				
				ship.gameObject.AddComponent<TrackerImperialPatrol>();

				// Initial ships don't warp in.
				if (Time.time > (startTime + 2f))
					ship.GetComponent<FTL>().DoFTLIn();
			}

			
		}


		


	}
}