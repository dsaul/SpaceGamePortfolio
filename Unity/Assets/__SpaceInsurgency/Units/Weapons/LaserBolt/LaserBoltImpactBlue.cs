using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public class LaserBoltImpactBlue : MonoBehaviour
	{
		public float despawnTime = float.MaxValue;

		void Update()
		{
			if (Time.time > despawnTime)
			{
				(Base.GetFromUniqueName(Base.kWeaponLaserBolt) as LaserBolt).ImpactPool.Abandon(gameObject);
				despawnTime = float.MaxValue;
			}
		}
	}
}