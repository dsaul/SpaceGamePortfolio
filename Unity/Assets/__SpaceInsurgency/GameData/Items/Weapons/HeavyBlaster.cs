using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	public class HeavyBlaster : Base
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
			Faction sourceFaction = source.GetComponent<Membership>().Faction;

			GameObject obj = GameObject.Instantiate(m_ProjectilePrefab, source.transform.position, Quaternion.identity) as GameObject;
			obj.transform.parent = transform;
			obj.transform.LookAt(target.transform);
			obj.name = string.Format("Missile - {0} - Blaster Heavy", sourceFaction.DisplayName);

			TargetedFollowingProjectile projectile = obj.GetComponent<TargetedFollowingProjectile>();
			projectile.SpawnParent = transform;
			projectile.Target = target;
			projectile.Source = source.GetComponent<SpaceObject>();
			projectile.SourceFaction = sourceFaction;
			projectile.TargetDetonateRange = TargetDetonateRange;
			projectile.AreaOfEffectRange = AreaOfEffectRange;
			projectile.Speed = ProjectileSpeed;
			projectile.Lifetime = ProjectileLifetime;
			projectile.Damage = Damage;
			projectile.AudioOnImpact = m_AudioOnImpact;
			obj.SetActive(true);

			AudioManager.Main.PlayClipAtPosition(m_FiringAudioClips, AudioManager.Main.mixerWeapons, source.transform.position);
		}
	}
}