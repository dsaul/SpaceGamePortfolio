using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SpaceInsurgency
{
	#if __DISABLEDAIEDIT__
	//[CustomEditor(typeof(AI))]
	public class AIEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			AI ai = target as AI;

			GUILayout.Label("Needs:");

			for (int i=0; i<ai.Needs.Count; i++)
			{
				AINeedsProvider anp = ai.Needs[i];

				GUILayout.Label(string.Format("{0}\n   Urgency: {1}", anp.GetType().ToString(), anp.Urgency));

				AINeedDestination nd = anp as AINeedDestination;
				if (null != nd)
				{
					for (int j = 0; j < nd.m_Waypoints.Count; j++)
					{
						GUILayout.Label(string.Format("   WP {0}: {1}", j, nd.m_Waypoints[j].ToString()));
					}
				}


				AINeedHostility nh = anp as AINeedHostility;
				if (null != nh)
				{
					EditorGUILayout.EnumPopup("   Chosen Atk Type:", nh.chosenAttackType);
					EditorGUILayout.FloatField("   Atk Type Time Left:", nh.chosenAttackTypeUntilTime - Time.time);
					
					GUILayout.Label(string.Format("   Dispositions ({0}):", nh.dispositions.Count));

					foreach (KeyValuePair<DynamicAgent, DispositionInfo> kvp in nh.dispositions)
					{
						EditorGUILayout.ObjectField("      Ship:", kvp.Key, typeof(DynamicAgent), true);
						EditorGUILayout.IntField("      Disposition:", kvp.Value.Disposition);
						GUILayout.Space(5f);
					}

					GUILayout.Label(string.Format("   \"Lowest Dispositions\" ({0}):", nh.lowestDispositions.Count));
					EditorGUILayout.Toggle("      Has Lowest Override:", nh.lowestDispositionOverride != null);
					EditorGUILayout.IntField("      Number:", nh.lowestDispositionNum);
					foreach (DispositionInfo di in nh.lowestDispositions)
					{
						EditorGUILayout.ObjectField("      Ship:", di.ship, typeof(DynamicAgent), true);
					}
					

				}

				AINeedCommerce nc = anp as AINeedCommerce;
				if (null != nc)
				{
					EditorGUILayout.ObjectField("      Chosen Planet Destination:", nc.chosenPlanetDestination, typeof(StaticAgent), true);

					if (null == nc.pointAroundChosenPlanet)
						EditorGUILayout.LabelField("      Point around planet:", "none");
					else
						EditorGUILayout.Vector3Field("      Point around planet:", nc.pointAroundChosenPlanet.Value);


				}

				

			}
		}

	}
#endif
}
