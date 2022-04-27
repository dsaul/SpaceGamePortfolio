using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SpaceInsurgency
{
	[CustomEditor(typeof(TargetedFollowingProjectile))]
	public class TargetedFollowingProjectileEditor : Editor
	{
		void OnSceneGUI()
		{
			TargetedFollowingProjectile tfp = target as TargetedFollowingProjectile;
			
			Handles.color = new Color(1f, 0f, 0f, 0.1f);
			Handles.DrawSolidDisc(tfp.transform.position, Vector3.up, tfp.TargetDetonateRange);
			
			Handles.color = new Color(1f, 1f, 0f, 0.1f);
			Handles.DrawSolidDisc(tfp.transform.position, Vector3.up, tfp.AreaOfEffectRange);
			
			

		}
	}
}