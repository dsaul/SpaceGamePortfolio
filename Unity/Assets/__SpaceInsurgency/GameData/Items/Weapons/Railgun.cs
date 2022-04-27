using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	public class Railgun : Base
	{
		#region Variables

		[SerializeField]
		List<AudioClip> m_FiringAudioClips;
		[Inspect, Group("Audio")]
		public List<AudioClip> FiringAudioClips
		{
			get { return m_FiringAudioClips; }
			set { m_FiringAudioClips = value; }
		}

		[SerializeField]
		GameObject m_ProjectilePrefab;
		[Inspect, Group("Weapon"), DontAllowSceneObject]
		public GameObject ProjectilePrefab
		{
			get { return m_ProjectilePrefab; }
			set { m_ProjectilePrefab = value; }
		}

		[SerializeField]
		List<AudioClip> m_AudioOnImpact;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioOnImpact
		{
			get { return m_AudioOnImpact; }
			set { m_AudioOnImpact = value; }
		}

		#endregion Variables
		
		public override bool IsActivationAppropriate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
			return true;
		}

		public override void OnActionClick()
		{
			//SelectionManager.Main.GroupFlipToggleOn(this.GetType());
#warning toggle enabled
		}

		protected override void Activate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
			Debug.Log("ActivateWeaponRailgun");

			Faction sourceFaction = source.GetComponent<Membership>().Faction;

			GameObject obj = GameObject.Instantiate(m_ProjectilePrefab, source.transform.position, Quaternion.identity) as GameObject;
			obj.transform.parent = transform;
			obj.name = string.Format("Missile - {0} - Railgun", sourceFaction.DisplayName);

			TargetedNonFollowingProjectile projectile = obj.GetComponent<TargetedNonFollowingProjectile>();
			projectile.SpawnParent = transform;
			projectile.Target = target.transform.position;
			projectile.Source = source.GetComponent<SpaceObject>();
			projectile.SourceFaction = sourceFaction;
			projectile.TargetDetonateRange = TargetDetonateRange;
			projectile.AreaOfEffectRange = AreaOfEffectRange;
			projectile.Damage = Damage;
			projectile.Speed = ProjectileSpeed;
			projectile.Lifetime = ProjectileLifetime;
			projectile.AudioOnImpact = m_AudioOnImpact;
			obj.SetActive(true);

			AudioManager.Main.PlayClipAtPosition(m_FiringAudioClips, AudioManager.Main.mixerWeapons, source.transform.position);
		}
	}
}