using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class AINeedGuard : AINeed2
	{
		public override int Importance // higher the more important
		{
			get
			{
				return 90;
			}
		}
	}
}
