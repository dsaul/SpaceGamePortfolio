using UnityEngine;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	public class HailChainGun : Base
	{
		public override bool IsActivationAppropriate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
#warning have these items fire only when appropriate to do so
			return true;
		}

		public override void OnActionClick()
		{
			//SelectionManager.Main.GroupFlipToggleOn(this.GetType());
#warning toggle enabled
		}

		protected override void Activate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{

#warning create weapon
		}
	}
}