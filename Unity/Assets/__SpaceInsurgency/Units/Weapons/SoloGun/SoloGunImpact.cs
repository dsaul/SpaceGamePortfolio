using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public class SoloGunImpact : MonoBehaviour
	{
		public float despawnTime = float.MaxValue;

		void Update()
		{
			if (Time.time > despawnTime)
			{
				(Base.GetFromUniqueName(Base.kWeaponSoloGun) as SoloGun).ImpactPool.Abandon(gameObject);
				despawnTime = float.MaxValue;
			}
		}
	}
}