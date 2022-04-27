using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[RequireComponent(typeof(LineRenderer))]
	public class SniperBeam : MonoBehaviour
	{
		public Transform SpawnParent;
		
		
		
		public Texture[] BeamFrames;    // Animation frame sequence
		public float FrameStep;         // Animation time

		public float beamScale;         // Default beam scale to be kept over distance
		public float MaxBeamLength;     // Maximum beam length

		public bool AnimateUV;          // UV Animation
		public float UVTime;            // UV Animation speed

		public Transform rayImpact;     // Impact transform
		public Transform rayMuzzle;     // Muzzle flash transform

		LineRenderer lineRenderer;      // Line rendered component
		RaycastHit hitPoint;            // Raycast structure

		int frameNo;                    // Frame counter
		int FrameTimerID;               // Frame timer reference
		float beamLength;               // Current beam length
		float initialBeamOffset;        // Initial UV offset 


		public float destroyTime = float.MaxValue;
		public SpaceObject source = null;
		public Faction sourceFaction = null;
		public float damage = 0f;

		void Awake()
		{
			// Get line renderer component
			lineRenderer = GetComponent<LineRenderer>();

			// Assign first frame texture
			if (!AnimateUV && BeamFrames.Length > 0)
				lineRenderer.material.mainTexture = BeamFrames[0];

			// Randomize uv offset
			initialBeamOffset = Random.Range(0f, 5f);
		}

		[ContextMenu("OnSpawned()")]
		// OnSpawned called by pool manager 
		public void OnSpawned()
		{
			// Do one time raycast
			Raycast();

			// Start animation sequence if beam frames array has more than 2 elements
			if (BeamFrames.Length > 1)
				Animate();

			destroyTime = Time.time + 1f;
		}

		[ContextMenu("OnDespawned()")]
		// OnDespawned called by pool manager 
		public void OnDespawned()
		{
			// Reset frame counter
			frameNo = 0;

			// Clear timer
			if (FrameTimerID != -1)
			{
				F3DTime.time.RemoveTimer(FrameTimerID);
				FrameTimerID = -1;
			}

		}

		// Hit point calculation
		void Raycast()
		{
			// Prepare structure and create ray
			hitPoint = new RaycastHit();
			Ray ray = new Ray(transform.position, transform.forward);

			// Calculate default beam proportion multiplier based on default scale and maximum length
			float propMult = MaxBeamLength * (beamScale / 10f);

			// Raycast
			if (Physics.Raycast(ray, out hitPoint, MaxBeamLength))
			{
				// Get current beam length and update line renderer accordingly
				beamLength = Vector3.Distance(transform.position, hitPoint.point);
				lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));

				// Calculate default beam proportion multiplier based on default scale and current length
				propMult = beamLength * (beamScale / 10f);

				if (hitPoint.collider.tag != Const.Tags.MovementPlane && 
					hitPoint.collider.tag != Const.Tags.Ignore)
				{
					SpaceObject spaceObject = hitPoint.collider.GetComponent<SpaceObject>();
					if (null != spaceObject)
					{
						spaceObject = spaceObject.Root;

						spaceObject.TakeDamage(damage, SpaceObject.DamageType.Regular);

						GameObject impact = (Base.GetFromUniqueName(Base.kWeaponSniperBeam) as Items.SniperBeam).ImpactPool.Get(null);
						impact.transform.parent = SpawnParent;

						impact.GetComponent<SniperImpact>().despawnTime = Time.time + 2f;

						impact.transform.position = hitPoint.point;
						impact.SetActive(true);

						//Debug.Log("Sniper Impact" + hitPoint.collider, hitPoint.collider);
					}
				}

				
				


				// Adjust impact effect position
				if (rayImpact)
					rayImpact.position = hitPoint.point - transform.forward * 0.5f;
			}

			// Nothing was hir
			else
			{
				// Set beam to maximum length
				beamLength = MaxBeamLength;
				lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));

				// Adjust impact effect position
				if (rayImpact)
					rayImpact.position = transform.position + transform.forward * beamLength;
			}

			// Adjust muzzle position
			if (rayMuzzle)
				rayMuzzle.position = transform.position + transform.forward * 0.1f;

			// Set beam scaling according to its length
			lineRenderer.material.SetTextureScale("_MainTex", new Vector2(propMult, 1f));
		}

		// Advance texture frame
		void OnFrameStep()
		{
			// Set current texture frame based on frame counter
			lineRenderer.material.mainTexture = BeamFrames[frameNo];
			frameNo++;

			// Reset frame counter
			if (frameNo == BeamFrames.Length)
				frameNo = 0;
		}

		// Initialize frame animation
		void Animate()
		{
			if (BeamFrames.Length > 1)
			{
				// Set current frame
				frameNo = 0;
				lineRenderer.material.mainTexture = BeamFrames[frameNo];

				// Add timer 
				FrameTimerID = F3DTime.time.AddTimer(FrameStep, BeamFrames.Length - 1, OnFrameStep);

				frameNo = 1;
			}
		}

		// Apply force to last hit object
		void ApplyForce(float force)
		{
			if (hitPoint.rigidbody != null)
				hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.VelocityChange);
		}

		void Update()
		{
			// Animate texture UV
			if (AnimateUV)
				lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time * UVTime + initialBeamOffset, 0f));

			if (Time.time > destroyTime)
			{
				(Base.GetFromUniqueName(Base.kWeaponSniperBeam) as Items.SniperBeam).BeamPool.Abandon(gameObject);
			}
		}
	}

}