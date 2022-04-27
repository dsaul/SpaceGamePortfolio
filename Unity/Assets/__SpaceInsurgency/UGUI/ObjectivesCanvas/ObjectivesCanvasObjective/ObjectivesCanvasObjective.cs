using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class ObjectivesCanvasObjective : MonoBehaviour
	{
		[Inspect]
		public Text text;

		[Inspect]
		public Image image;
	}
}