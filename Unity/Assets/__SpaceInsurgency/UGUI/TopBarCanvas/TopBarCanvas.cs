using UnityEngine;
using System.Collections;
using SharedCode;
using AdvancedInspector;
using J = SpaceInsurgency.Journal.Journal;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class TopBarCanvas : SharedCode.Behaviours.InstanceTracked<TopBarCanvas>
	{
		protected override void Start()
		{
			base.Start();

			MakeHidden();
		}

		public void MakeVisible()
		{
			GetComponent<Animator>().SetBool("visible", true);
		}

		public void MakeHidden()
		{
			GetComponent<Animator>().SetBool("visible", false);
		}
	}
}