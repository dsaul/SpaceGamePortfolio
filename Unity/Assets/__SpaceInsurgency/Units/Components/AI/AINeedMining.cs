using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class AINeedMining : AINeed2
	{
		public override int Importance // higher the more important
		{
			get
			{
				return int.MinValue;
			}
		}
	}
}
