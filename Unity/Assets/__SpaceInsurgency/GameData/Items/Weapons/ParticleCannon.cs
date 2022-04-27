using UnityEngine;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	public class ParticleCannon : Base
	{
		#region Events

		#endregion Events
#if false
		private ArcReactor_Arc arc = null;

		private GameObject laserObject = null;

		private SpaceObject lastActivateSource = null;

		private SpaceObject lastActivateTarget = null;


		public override bool IsActivationAppropriate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
			// Should only fire if we are facing the direction of the target.
			Vector3 localTargetPos = source.transform.InverseTransformPoint(target.transform.position);
			Quaternion rot = Quaternion.LookRotation(localTargetPos);
			Vector3 eulerRot = rot.eulerAngles;

			bool withinCone = eulerRot.y < 30 || eulerRot.y > (360 - 30);

			return withinCone;
		}

		public override void OnActionClick()
		{
			//SelectionManager.Main.GroupFlipToggleOn(this.GetType());
#warning toggle enabled
		}

		protected override void Activate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
			// If there isn't already a laser being fired from this item, fire one.
			if (null == laserObject)
			{
				// Get one of the attachment points on the ship to fire from.
				Transform shipAttachmentPoint = null;
				if (null != source.GetComponent<DynamicAgent>())
				{
					Transform[] attachmentPoints = source.GetComponent<DynamicAgent>().Parts.laserAttachmentPoints;
					if (null != attachmentPoints && 0 != attachmentPoints.Length)
					{
						shipAttachmentPoint = attachmentPoints[UnityEngine.Random.Range(0, attachmentPoints.Length)];
					}
				}

				// If we don't have an attachment point use the ship's transform itself.
				if (null == shipAttachmentPoint)
				{
					Debug.LogWarning("There exists no laser attachment point for this model.", source);
					shipAttachmentPoint = source.transform;
				}

				// Create the effect object.
				laserObject = GameObject.Instantiate(AssetManager.Main.items.arcLaserCannonParticle) as GameObject;
				arc = laserObject.GetComponent<ArcReactor_Arc>();
				arc.shapeTransforms[0] = shipAttachmentPoint;
				arc.shapeTransforms[1] = target.transform;

				lastActivateSource = source.GetComponent<SpaceObject>();
				lastActivateTarget = target;

				target.TakeDamage(Damage, SpaceObject.DamageType.Regular);
			}
		}

		protected override void FixedUpdate()
		{
			// We need to check to see if we need to stop the laser effect prematurely. This happens if we are no longer facing the target.
			if (null == laserObject)
				return;

			float sqrDistance = lastActivateSource.transform.position.SqrDistance(lastActivateTarget.transform.position);

			if (false == IsActivationAppropriate(lastActivateSource, lastActivateTarget, sqrDistance))
			{
				GameObject.Destroy(laserObject);
				laserObject = null;
				arc = null;
			}
		}

		public override void OnRemovedFrom(EquippableComponent newContainer)
		{
			if (null != laserObject)
			{
				GameObject.Destroy(laserObject);
			}
			laserObject = null;
			arc = null;
		}
#endif
	}
}