using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class LaserImpulseImpactBlue : MonoBehaviour
	{
		public float despawnTime = float.MaxValue;

		void Update()
		{
			if (Time.time > despawnTime)
			{
#warning incomplete
				//AssetManager.Main.weapons.soloGunImpactPool.Abandon(gameObject);
				despawnTime = float.MaxValue;
			}
		}
	}
}