using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	public class LaserBolt : Base
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
		AutoObjectPool<GameObject, object> m_ImpactPool;

		public AutoObjectPool<GameObject, object> ImpactPool
		{
			get { return m_ImpactPool; }
			set { m_ImpactPool = value; }
		}

		[SerializeField]
		GameObject m_ImpactPrefab;
		[Inspect, Group("Weapon"), DontAllowSceneObject]
		public GameObject ImpactPrefab
		{
			get { return m_ImpactPrefab; }
			set { m_ImpactPrefab = value; }
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
		#region Setup

		protected override void Start()
		{
			base.Start();

			m_ImpactPool = new AutoObjectPool<GameObject, object>(
				delegate(object a1) {
					GameObject obj = GameObject.Instantiate(m_ImpactPrefab) as GameObject;
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;

					ParticleSystem[] ps = obj.GetComponentsInChildren<ParticleSystem>();
					for (int i = 0; i < ps.Length; i++)
						ps[i].Simulate(0, true, true);

					return obj;
				},
				delegate(GameObject obj) {
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;

					ParticleSystem[] ps = obj.GetComponentsInChildren<ParticleSystem>();
					for (int i = 0; i < ps.Length; i++)
						ps[i].Simulate(0, true, true);
				});
			m_ImpactPool.PreloadObjects(null);
		}

		#endregion Setup

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
			obj.GetComponent<LaserBoltProjectileBlue>().SpawnParent = transform;
			obj.transform.parent = transform;
			obj.transform.LookAt(target.transform);
			obj.name = string.Format("TargetedNonFollowingProjectile - {0} - ItemImpWeaponLaserBolt", sourceFaction.DisplayName);

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