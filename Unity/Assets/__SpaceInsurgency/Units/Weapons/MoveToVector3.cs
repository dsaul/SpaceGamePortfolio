using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class MoveToVector3 : MonoBehaviour
	{
		#region Setup

		public Vector3 destination;
		public float speed = 20f;

		#endregion Setup
		#region Main

		void Update()
		{
			if (Const.Vector3.Sentinel == destination)
				return;
			if (Vector3.zero == destination)
				return;

			float step = speed * Time.deltaTime;

			transform.position = Vector3.MoveTowards(transform.position, destination, step);
		}

		#endregion Main
	}
}