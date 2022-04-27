using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	public class SniperBeam : Base
	{
		#region Variables

		[SerializeField]
		List<AudioClip> m_FiringAudioClips;

		[Inspect, Group("Sound")]
		public List<AudioClip> FiringAudioClips
		{
			get { return m_FiringAudioClips; }
			set { m_FiringAudioClips = value; }
		}

		AutoObjectPool<GameObject, object> m_BeamPool;

		public AutoObjectPool<GameObject, object> BeamPool
		{
			get { return m_BeamPool; }
			set { m_BeamPool = value; }
		}


		AutoObjectPool<GameObject, object> m_ImpactPool;

		public AutoObjectPool<GameObject, object> ImpactPool
		{
			get { return m_ImpactPool; }
			set { m_ImpactPool = value; }
		}

		[SerializeField]
		GameObject m_BeamPrefab;
		[Inspect, Group("Weapon"), DontAllowSceneObject]
		public GameObject BeamPrefab
		{
			get { return m_BeamPrefab; }
			set { m_BeamPrefab = value; }
		}

		[SerializeField]
		GameObject m_ImpactPrefab;
		[Inspect, Group("Weapon"), DontAllowSceneObject]
		public GameObject ImpactPrefab
		{
			get { return m_ImpactPrefab; }
			set { m_ImpactPrefab = value; }
		}


		#endregion Variables
		#region Events

		protected override void Start()
		{
			base.Start();

			m_BeamPool = new AutoObjectPool<GameObject, object>(
				delegate(object a1) // generator
				{
					GameObject obj = GameObject.Instantiate(m_BeamPrefab) as GameObject;
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;

					SpaceInsurgency.SniperBeam sb = obj.GetComponent<SpaceInsurgency.SniperBeam>();
					sb.SpawnParent = null;
					sb.destroyTime = float.MaxValue;
					sb.source = null;
					sb.sourceFaction = null;
					sb.damage = 0f;

					return obj;
				},
				delegate(GameObject obj) // cleanup
				{
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;

					SpaceInsurgency.SniperBeam sb = obj.GetComponent<SpaceInsurgency.SniperBeam>();
					sb.SpawnParent = null;
					sb.OnDespawned();
					sb.destroyTime = float.MaxValue;
					sb.source = null;
					sb.sourceFaction = null;
					sb.damage = 0f;
				});
			m_BeamPool.PreloadObjects(null);

			m_ImpactPool = new AutoObjectPool<GameObject, object>(
				delegate(object a1) // generator
				{
					GameObject obj = GameObject.Instantiate(m_ImpactPrefab) as GameObject;
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;

					ParticleSystem ps = obj.GetComponent<ParticleSystem>();
					ps.Simulate(0, true, true);
					return obj;
				},
				delegate(GameObject obj) // cleanup
				{
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;

					ParticleSystem ps = obj.GetComponent<ParticleSystem>();
					ps.Simulate(0, true, true);
				});
			m_ImpactPool.PreloadObjects(null);
		}

		#endregion Events

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

			GameObject obj = m_BeamPool.Get(null);
			obj.transform.parent = transform;

			obj.transform.position = source.transform.position;
			obj.transform.LookAt(target.transform);
			obj.name = string.Format("Beam - {0} - Sniper Beam", sourceFaction.DisplayName);
			obj.SetActive(true);

			SpaceInsurgency.SniperBeam sb = obj.GetComponent<SpaceInsurgency.SniperBeam>();
			sb.SpawnParent = transform;
			sb.source = source.GetComponent<SpaceObject>();
			sb.sourceFaction = sourceFaction;
			sb.damage = Damage;
			sb.OnSpawned();

			AudioManager.Main.PlayClipAtPosition(m_FiringAudioClips, AudioManager.Main.mixerWeapons, source.transform.position);
		}
	}
}