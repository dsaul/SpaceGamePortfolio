using UnityEngine;
using System.Collections;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	public class ScreenFaderCanvas : SharedCode.Behaviours.InstanceTracked<ScreenFaderCanvas>
	{
		[Inspect]
		public void GoToClear()
		{
			GetComponent<Animator>().SetBool("clear", true);
		}

		[Inspect]
		public void GoToBlack()
		{
			GetComponent<Animator>().SetBool("clear", false);
		}
	}
}