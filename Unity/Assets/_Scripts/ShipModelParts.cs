using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class ShipModelParts : SharedCode.Behaviours.Base
	{
		#region Variables

		[SerializeField]
		List<MeshRenderer> m_FadeRenderers = new List<MeshRenderer>();

		[Inspect, Group("Fading")]
		public List<MeshRenderer> FadeRenderers
		{
			get { return m_FadeRenderers; }
			set { m_FadeRenderers = value; }
		}

		
		

		[Inspect]
		public bool debug = false;

		[Inspect, Group("Engines")]
		public float navMeshAgentRadius = 1f;

		// Nothing starts without the ship being set.
		DynamicAgent m_Ship = null;

		[Inspect, Group("Runtime"), ReadOnly]
		public DynamicAgent Ship
		{
			get
			{
				return m_Ship;
			}
			set
			{
				if (null != m_Ship)
				{
					m_Ship.GetComponent<Engines>().OnMovement -= OnMovement;
					m_Ship.GetComponent<Engines>().OnRotateLeft -= OnRotateLeft;
					m_Ship.GetComponent<Engines>().OnRotateRight -= OnRotateRight;
				}

				m_Ship = value;

				if (null != m_Ship)
				{
					m_Ship.GetComponent<Engines>().OnMovement += OnMovement;
					m_Ship.GetComponent<Engines>().OnRotateLeft += OnRotateLeft;
					m_Ship.GetComponent<Engines>().OnRotateRight += OnRotateRight;
				}
			}
		}

		#endregion Variables
		#region Events

		protected override void Start()
		{
			base.Start();
			
			if (null != thrusterAudioSource)
			{
				thrusterAudioSource.volume = 0f;
				thrusterAudioSource.enabled = false;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			Ship = null;
		}

		#endregion Events
		#region Utility

		[Inspect, Group("Utility")]
		public void DisableVisuals()
		{
			gameObject.SetActive(false);
		}

		[Inspect, Group("Utility")]
		public void EnableVisuals()
		{
			gameObject.SetActive(true);
		}

		[Inspect, Group("Utility")]
		public void DisableColliders()
		{
			Collider[] colliders = GetComponentsInChildren<Collider>();
			for (int i = 0; i < colliders.Length; i++)
				colliders[i].enabled = false;
		}

		[Inspect, Group("Utility")]
		public void EnableColliders()
		{
			Collider[] colliders = GetComponentsInChildren<Collider>();
			for (int i = 0; i < colliders.Length; i++)
				colliders[i].enabled = true;
		}

		#endregion Utility
		#region Shield

		[Inspect, Group("Shield")]
		public Transform shield;

		#endregion Shield
		#region Lasers

		[Inspect, Group("Lasers")]
		public Transform[] laserAttachmentPoints;

		#endregion Lasers
		#region Selection

		[Inspect, Group("Selection")]
		public GameObject baseObjectForSelectionCalculation;

		#endregion Selection
		#region Sound FX

		[Inspect, Group("Sound FX")]
		public AudioSource thrusterAudioSource;

		float thrusterAudioVolumeMax = 1f;
		float thrusterAudioVolumeMin = 0f;

		float thrusterAudioVolumeTarget = 0f;

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			
			if (null == m_Ship)
				return;
			if (null == thrusterAudioSource || null == thrusterAudioSource.clip)
				return;

			bool playAudio = rearThrusterThrottleTarget != rearThrusterThrottleMin || leftThrusterThrottleTarget != leftThrusterThrottleMin || rightThrusterThrottleTarget != rightThrusterThrottleMin;

			thrusterAudioVolumeTarget = playAudio ? thrusterAudioVolumeMax : thrusterAudioVolumeMin;
			thrusterAudioSource.volume = thrusterAudioSource.volume.MoveTowards(thrusterAudioVolumeTarget, 3f * Time.fixedDeltaTime);
			thrusterAudioSource.enabled = 0 != thrusterAudioSource.volume;

		}

		#endregion Sound FX
		#region Thrusters

		[Inspect, Group("Thrusters")]
		public bool toggleRearThrusters = true;
		[Inspect, Group("Thrusters")]
		public SgtThruster[] rearThrusters;

		[Inspect, Group("Thrusters")]
		public float rearThrusterThrottleMax = 1f;
		[Inspect, Group("Thrusters")]
		public float rearThrusterThrottleMin = 0f;

		float rearThrusterThrottleTarget = 0f;

		void OnMovement(Engines source, bool isMoving, bool startedMoving, bool stillMoving, bool endingMoving)
		{
			if (debug)
				Debug.Log("rear thrusters " + isMoving);
			
			if (false == toggleRearThrusters)
				return;

			rearThrusterThrottleTarget = true == isMoving ? rearThrusterThrottleMax : rearThrusterThrottleMin;

			for (int i = 0; i < rearThrusters.Length; i++)
			{
				SgtThruster t = rearThrusters[i];
				if (null == t)
					continue;
				t.Throttle = t.Throttle.MoveTowards(rearThrusterThrottleTarget, 1.5f * Time.fixedDeltaTime);
				t.enabled = 0 != t.Throttle;
			}
		}

		[Inspect, Group("Thrusters")]
		public bool toggleLeftThrusters = true;
		[Inspect, Group("Thrusters")]
		public SgtThruster[] leftThrusters;

		[Inspect, Group("Thrusters")]
		public float leftThrusterThrottleMax = 1f;
		[Inspect, Group("Thrusters")]
		public float leftThrusterThrottleMin = 0f;

		float leftThrusterThrottleTarget = 0f;

		void OnRotateLeft(Engines source, bool isRotatingLeft, bool startedRotatingLeft, bool stillRotatingLeft, bool endingRotatingLeft)
		{
			if (false == toggleLeftThrusters)
				return;

			leftThrusterThrottleTarget = true == isRotatingLeft ? leftThrusterThrottleMax : leftThrusterThrottleMin;

			for (int i = 0; i < leftThrusters.Length; i++)
			{
				SgtThruster t = leftThrusters[i];
				if (null == t)
					continue;
				t.Throttle = t.Throttle.MoveTowards(leftThrusterThrottleTarget, 1.5f * Time.fixedDeltaTime);
				t.enabled = 0 != t.Throttle;
			}
		}

		[Inspect, Group("Thrusters")]
		public bool toggleRightThrusters = true;
		[Inspect, Group("Thrusters")]
		public SgtThruster[] rightThrusters;

		[Inspect, Group("Thrusters")]
		public float rightThrusterThrottleMax = 1f;
		[Inspect, Group("Thrusters")]
		public float rightThrusterThrottleMin = 0f;

		float rightThrusterThrottleTarget = 0f;

		void OnRotateRight(Engines source, bool isRotatingRight, bool startedRotatingRight, bool stillRotatingRight, bool endingRotatingRight)
		{
			if (false == toggleRightThrusters)
				return;

			rightThrusterThrottleTarget = true == isRotatingRight ? rightThrusterThrottleMax : rightThrusterThrottleMin;

			for (int i = 0; i < rightThrusters.Length; i++)
			{
				SgtThruster t = rightThrusters[i];
				if (null == t)
					continue;
				t.Throttle = t.Throttle.MoveTowards(rightThrusterThrottleTarget, 1.5f * Time.fixedDeltaTime);
				t.enabled = 0 != t.Throttle;
			}
		}

		#endregion Thrusters
		#region Destruction

		[SerializeField]
		bool m_HasDestructionAoEDamage = true;
		[Inspect, Group("Destruction")]
		public bool HasDestructionAoEDamage
		{
			get { return m_HasDestructionAoEDamage; }
			set { m_HasDestructionAoEDamage = value; }
		}


		[SerializeField]
		float m_DestructionAoEDamageRange = 4f;
		[Inspect, Group("Destruction")]
		public float DestructionAoEDamageRange
		{
			get { return m_DestructionAoEDamageRange; }
			set { m_DestructionAoEDamageRange = value; }
		}

		[SerializeField]
		float m_DestructionAoEDamageAmount = 40f;
		[Inspect, Group("Destruction")]
		public float DestructionAoEDamageAmount
		{
			get { return m_DestructionAoEDamageAmount; }
			set { m_DestructionAoEDamageAmount = value; }
		}

		#endregion Destruction
	}
}