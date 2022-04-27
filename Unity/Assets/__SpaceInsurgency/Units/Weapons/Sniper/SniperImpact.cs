using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public class SniperImpact : MonoBehaviour
	{
		public float despawnTime = float.MaxValue;
		
		void Update()
		{
			if (Time.time > despawnTime)
			{
				(Base.GetFromUniqueName(Base.kWeaponSniperBeam) as Items.SniperBeam).ImpactPool.Abandon(gameObject);
				despawnTime = float.MaxValue;
			}
		}
	}
}