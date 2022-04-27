using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public class SpawnerTradeConvoy : SharedCode.Behaviours.Base
	{
		float m_NextSpawn = float.MinValue;
		public Transform[] spawnPoints;
		

		[Restrict("ListOfDynamicAgentDefinitionsForEditor")]
		static string dynamicAgentDefinition = DynamicAgentDefinition.kDynamicAgentDefinitionSmallTransport;

		public List<string> ListOfDynamicAgentDefinitionsForEditor()
		{
			return DynamicAgentDefinition.kListOfDynamicAgentDefinitionsForEditor;
		}

		public static DynamicAgent SpawnTradeShip(Vector3 _position, Quaternion _rotation)
		{
			DynamicAgent.Serialized serialized = new DynamicAgent.Serialized();
			serialized.dynamicAgentDefinition = dynamicAgentDefinition;
			serialized.position = _position;
			serialized.rotation = _rotation;
			

			serialized.membership.faction = Faction.kFactionTrader;

			serialized.weapons.items.Add(Base.kWeaponLightBlaster);

			serialized.ai.needs.Add(typeof(AINeedDestination).FullName);
			//serialized.ai.needs.Add(typeof(AINeedHostility).FullName);
			serialized.ai.needs.Add(typeof(AINeedCommerce).FullName);

			DynamicAgent ship = DynamicAgentDefinition.Spawn(serialized);
			if (null == ship)
				return null;
			
			return ship;
		}

		protected override void Start()
		{
			base.Start();

			
		}

		protected override void Update()
		{
			base.Update();

			if (Time.time < m_NextSpawn)
				return;

			Transform chosenSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

			DynamicAgent ship = SpawnTradeShip(chosenSpawnPoint.position, chosenSpawnPoint.rotation);
			if (null != ship)
			{
				ship.GetComponent<FTL>().DoFTLIn();
			}
			

#warning once we implement guarding, add guard ships

			m_NextSpawn = Time.time + UnityEngine.Random.Range(15, 60);
		}
	}
}
