using SharedCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInspector;

namespace SpaceInsurgency
{
	
	[AdvancedInspector]
	public class TargetedFollowingProjectile : SharedCode.Behaviours.InstanceTracked<TargetedFollowingProjectile>
	{
		#region Event Definitions

		public event Action<TargetedFollowingProjectile> OnDetonate; // TargetedFollowingProjectile source

		#endregion Event Definitions
		#region Variables

		[SerializeField]
		List<AudioClip> m_AudioOnImpact;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioOnImpact
		{
			get { return m_AudioOnImpact; }
			set { m_AudioOnImpact = value; }
		}

		[SerializeField]
		Transform m_SpawnParent;
		[Inspect, ReadOnly, Group("Editor")]
		public Transform SpawnParent
		{
			get { return m_SpawnParent; }
			set { m_SpawnParent = value; }
		}

		#endregion Variables
		#region Setup



		protected override void Awake()
		{
			base.Awake();

			OnDetonate = new Action<TargetedFollowingProjectile>(DefaultOnDetonate);
		}

		protected override void Start()
		{
			base.Start();
			
			m_SpawnTime = Time.time;

			if (true == HasTrail)
			{
				m_ProjectileTrail = GameObject.Instantiate(AssetManager.Main.ProjectileTrailEnergy) as GameObject;
				m_ProjectileTrail.transform.parent = m_SpawnParent;
				TrailRenderer trailRenderer = m_ProjectileTrail.GetComponent<TrailRenderer>();
				trailRenderer.startWidth = TrailStartWidth;
				trailRenderer.endWidth = TrailEndWidth;
				trailRenderer.time = TrailTime;
				trailRenderer.material.SetColor("_TintColor", TrailTintColor);
				TrackTransformPosition ttp = m_ProjectileTrail.GetComponent<TrackTransformPosition>();
				ttp.toTrack = transform;
				ttp.DoTrack();
				m_ProjectileTrail.SetActive(true);
			}
		}

		#endregion Setup

		#region Ancessor

		[Inspect, ReadOnly]
		private float m_AreaOfEffectRange = 6f;

		[Inspect, ReadOnly]
		private float m_Damage = 0f;

		[Inspect, ReadOnly]
		private Vector3 m_LastTargetPosition = Const.Vector3.Sentinel;

		[Inspect, ReadOnly]
		private float m_Lifetime = 10f;

		[SerializeField]
		private GameObject m_ProjectileTrail;

		[SerializeField]
		private SpaceObject m_Source;

		[Inspect, ReadOnly]
		private Faction m_SourceFaction = Faction.NeutralFaction;

		[SerializeField]
		private float m_SpawnTime;

		[Inspect, ReadOnly]
		private float m_Speed = 20f;

		[SerializeField]
		private SpaceObject m_Target;

		[Inspect, ReadOnly]
		private float m_TargetDetonateRange = 0.1f;

		[SerializeField]
		private bool m_HasTrail = true;

		[SerializeField]
		private float m_TrailEndWidth = 0.2f;

		[SerializeField]
		private float m_TrailStartWidth = 0.2f;

		[SerializeField]
		private float m_TrailTime = 0.15f;

		[SerializeField]
		private Color m_TrailTintColor = new Color(1f, 0.3529411764705882f, 0f);

		public float AreaOfEffectRange
		{
			get
			{
				return m_AreaOfEffectRange;
			}
			set
			{
				m_AreaOfEffectRange = value;
			}
		}

		public float Damage
		{
			get
			{
				return m_Damage;
			}
			set
			{
				m_Damage = value;
			}
		}
		[Inspect]
		public bool HasTrail
		{
			get
			{
				return m_HasTrail;
			}
			set
			{
				m_HasTrail = value;
			}
		}

		public Vector3 LastTargetPosition
		{
			get
			{
				return m_LastTargetPosition;
			}
			set
			{
				m_LastTargetPosition = value;
			}
		}

		public float Lifetime
		{
			get
			{
				return m_Lifetime;
			}
			set
			{
				m_Lifetime = value;
			}
		}

		[Inspect]
		public GameObject ProjectileTrail
		{
			get
			{
				return m_ProjectileTrail;
			}
		}
		[Inspect]
		public SpaceObject Source
		{
			get
			{
				return m_Source;
			}
			set
			{
				m_Source = value;
			}
		}

		public Faction SourceFaction
		{
			get
			{
				return m_SourceFaction;
			}
			set
			{
				m_SourceFaction = value;
			}
		}
		[Inspect]
		public float SpawnTime
		{
			get
			{
				return m_SpawnTime;
			}
		}

		public float Speed
		{
			get
			{
				return m_Speed;
			}
			set
			{
				m_Speed = value;
			}
		}
		[Inspect]
		public SpaceObject Target
		{
			get
			{
				return m_Target;
			}
			set
			{
				m_Target = value;
			}
		}

		public float TargetDetonateRange
		{
			get
			{
				return m_TargetDetonateRange;
			}
			set
			{
				m_TargetDetonateRange = value;
			}
		}
		[Inspect]
		public float TrailEndWidth
		{
			get
			{
				return m_TrailEndWidth;
			}
			set
			{
				m_TrailEndWidth = value;
			}
		}
		[Inspect]
		public float TrailStartWidth
		{
			get
			{
				return m_TrailStartWidth;
			}
			set
			{
				m_TrailStartWidth = value;
			}
		}
		[Inspect]
		public float TrailTime
		{
			get
			{
				return m_TrailTime;
			}
			set
			{
				m_TrailTime = value;
			}
		}
		[Inspect]
		public Color TrailTintColor
		{
			get
			{
				return m_TrailTintColor;
			}
			set
			{
				m_TrailTintColor = value;
			}
		}

		#endregion Ancessor

		#region Main

		private void DefaultOnDetonate(TargetedFollowingProjectile source)
		{
			AudioManager.Main.PlayClipAtPosition(m_AudioOnImpact, AudioManager.Main.mixerDestruction, transform.position);
			
			if (0 != m_Damage)
			{
				Vector3 thisPos = transform.position;

				List<DynamicAgent> ships = DynamicAgent.Instances;
				for (int i = 0; i < ships.Count; i++)
				{
					DynamicAgent ship = ships[i];
					if (thisPos.SqrDistance(ship.transform.position) < m_AreaOfEffectRange * m_AreaOfEffectRange)
						ship.GetComponent<SpaceObject>().TakeDamage(m_Damage, SpaceObject.DamageType.Regular);
				}
			}

			if (null != m_ProjectileTrail)
			{
				m_ProjectileTrail.GetComponent<TrailRenderer>().autodestruct = true;
			}
			GameObject.Destroy(gameObject);
		}

		protected override void Update()
		{
			base.Update();

			if (null != m_Target)
			{
				m_LastTargetPosition = m_Target.transform.position;
			}
			if (Const.Vector3.Sentinel == m_LastTargetPosition)
				return;

			if (Time.time > (m_SpawnTime + m_Lifetime))
			{
				OnDetonate(this);
				return;
			}

			if (transform.position == m_LastTargetPosition)
			{
				OnDetonate(this);
				return;
			}

			if (transform.position.SqrDistance(m_LastTargetPosition) < m_TargetDetonateRange * m_TargetDetonateRange)
			{
				OnDetonate(this);
				return;
			}

			// Check to see if we're intersecting anything with a collider.
			Collider[] hits = Physics.OverlapSphere(transform.position, 0.1f);
			for (int i = 0; i < hits.Length; i++)
			{
				// Things we don't want to match against, that require colliders however.
				if (hits[i].tag == Const.Tags.MovementPlane ||
					hits[i].tag == Const.Tags.Ignore)
					continue;

				SpaceObject spaceObject = hits[i].GetComponent<SpaceObject>();
				if (null == spaceObject)
					continue;

				spaceObject = spaceObject.Root;
				if (spaceObject == Source)
					continue;

				OnDetonate(this);
				return;
			}

			float step = m_Speed * Time.deltaTime;

			transform.position = Vector3.MoveTowards(transform.position, m_LastTargetPosition, step);
		}

		#endregion Main
	}
}