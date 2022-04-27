using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public class VulcanImpact : MonoBehaviour
	{
		public float despawnTime = float.MaxValue;

		void Update()
		{
			if (Time.time > despawnTime)
			{
				(Base.GetFromUniqueName(Base.kWeaponVulcan) as Vulcan).ImpactPool.Abandon(gameObject);
				despawnTime = float.MaxValue;
			}
		}
	}
}