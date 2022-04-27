using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class SpawnerSensorBoey : MonoBehaviour
	{
		[Restrict("ListOfFactionsForEditor")]
		[Inspect]
		public string factionUniqueName;
		public List<string> ListOfFactionsForEditor()
		{
			return Faction.kListOfFactionsForEditor;
		}
		

		void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 1);
		}

		void Awake()
		{
			LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;
		}

		void OnDestroy()
		{
			LevelGeometry.OnLevelGeometryCreated -= LevelGeometry_OnLevelGeometryCreated;
		}

		void LevelGeometry_OnLevelGeometryCreated(LevelGeometry source, bool fromSaveFile)
		{
			//Debug.Log("Spawner Sensor Boey LevelGeometry_OnLevelGeometryCreated");

			if (true == fromSaveFile)
			{
				Destroy(gameObject);
				return;
			}

			DynamicAgent.Serialized serialized = new DynamicAgent.Serialized();
			serialized.dynamicAgentDefinition = DynamicAgentDefinition.kDynamicAgentDefinitionSensorBoey;
			serialized.position = transform.position;
			serialized.rotation = Quaternion.identity;

			serialized.membership.faction = factionUniqueName;

			

			DynamicAgentDefinition.Spawn(serialized);
			

		}
	}
}