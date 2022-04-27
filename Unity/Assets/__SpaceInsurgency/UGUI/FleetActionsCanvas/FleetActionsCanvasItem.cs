using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class FleetActionsCanvasItem : SharedCode.Behaviours.InstanceTracked<FleetActionsCanvasItem>
	{
		public Action OnClickAction;
		public Func<bool> CheckEnableFunc; // returns is interactable

		public void ClickCallback()
		{
			if (null != OnClickAction)
				OnClickAction();
		}

		[Inspect]
		public void CheckInteractableNow()
		{
			GetComponent<UnityEngine.UI.Button>().interactable = CheckEnableFunc();
		}
	}
}