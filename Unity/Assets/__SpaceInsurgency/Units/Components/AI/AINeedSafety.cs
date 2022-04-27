using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class AINeedSafety : AINeed2
	{
		public override int Importance // higher the more important
		{
			get
			{
				return 100;
			}
		}
	}
}
