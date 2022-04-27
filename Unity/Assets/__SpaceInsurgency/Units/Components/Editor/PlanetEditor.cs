using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SpaceInsurgency
{
	[CustomEditor(typeof(StaticAgent))]
	public class PlanetEditor : Editor
	{
		void OnSceneGUI()
		{
			StaticAgent planet = target as StaticAgent;

			Handles.color = new Color(1f, 0f, 0f, 0.1f);
			Handles.DrawSolidDisc(planet.transform.position, Vector3.up, planet.IdleAndSpawnDistanceMin);
			Handles.color = Color.red;
			planet.IdleAndSpawnDistanceMin = Handles.ScaleValueHandle(planet.IdleAndSpawnDistanceMin,
							planet.transform.position + new Vector3(planet.IdleAndSpawnDistanceMin, 0, 0),
							Quaternion.identity,
							HandleUtility.GetHandleSize(planet.transform.position),
							Handles.CylinderCap,
							2);

			Handles.color = new Color(1f, 1f, 0f, 0.1f);
			Handles.DrawSolidDisc(planet.transform.position, Vector3.up, planet.IdleAndSpawnDistanceMax);
			Handles.color = Color.yellow;
			planet.IdleAndSpawnDistanceMax = Handles.ScaleValueHandle(planet.IdleAndSpawnDistanceMax,
							planet.transform.position + new Vector3(planet.IdleAndSpawnDistanceMax, 0, 0),
							Quaternion.identity,
							HandleUtility.GetHandleSize(planet.transform.position),
							Handles.CylinderCap,
							2);
			
		}
	}
}