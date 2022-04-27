using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class AINeedPirate : AINeed2
	{
		public override int Importance // higher the more important
		{
			get
			{
				return 49;
			}
		}
	}
}
