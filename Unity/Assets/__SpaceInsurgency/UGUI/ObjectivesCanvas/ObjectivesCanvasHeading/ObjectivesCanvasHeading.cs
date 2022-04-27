using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class ObjectivesCanvasHeading : MonoBehaviour
	{
		[Inspect]
		public Text text;
	}
}