using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class LaserBoltProjectileBlue : MonoBehaviour
	{
		#region Variables

		Transform m_SpawnParent;
		[Inspect, ReadOnly]
		public Transform SpawnParent
		{
			get { return m_SpawnParent; }
			set { m_SpawnParent = value; }
		}

		#endregion Variables
		
		void Start()
		{
			GetComponent<TargetedNonFollowingProjectile>().OnDetonate += LaserBoltProjectileBlue_OnDetonate;
		}

		void LaserBoltProjectileBlue_OnDetonate(TargetedNonFollowingProjectile source, SpaceObject collidedSpaceObject)
		{
			//Debug.Log("SoloGunProjectile_OnDetonate");
			GameObject obj = (Base.GetFromUniqueName(Base.kWeaponLaserBolt) as LaserBolt).ImpactPool.Get(null);
			obj.transform.position = transform.position;
			obj.transform.parent = m_SpawnParent;

			obj.GetComponent<LaserBoltImpactBlue>().despawnTime = Time.time + 1f;

			obj.SetActive(true);
		}
	}
}