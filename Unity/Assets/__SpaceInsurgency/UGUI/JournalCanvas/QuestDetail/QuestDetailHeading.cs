using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Journal
{
	[AdvancedInspector]
	public class QuestDetailHeading : MonoBehaviour
	{
		[Inspect]
		public Text text;
	}
}