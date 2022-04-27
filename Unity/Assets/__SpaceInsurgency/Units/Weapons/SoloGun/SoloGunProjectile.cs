using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class SoloGunProjectile : MonoBehaviour
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
			GetComponent<TargetedNonFollowingProjectile>().OnDetonate += SoloGunProjectile_OnDetonate;
		}

		void SoloGunProjectile_OnDetonate(TargetedNonFollowingProjectile source, SpaceObject collidedSpaceObject)
		{
			//Debug.Log("SoloGunProjectile_OnDetonate");
			GameObject obj = (Base.GetFromUniqueName(Base.kWeaponSoloGun) as SoloGun).ImpactPool.Get(null);
			obj.transform.position = transform.position;
			obj.transform.parent = m_SpawnParent;

			obj.GetComponent<SoloGunImpact>().despawnTime = Time.time + 0.53f;

			obj.SetActive(true);
		}
	}

}