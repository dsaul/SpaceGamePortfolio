using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpaceInsurgency;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency.Journal
{
	[AdvancedInspector]
	public class QuestDetailReward : MonoBehaviour
	{
		#region Variables

		[Inspect]
		public Text labelField;

		[Inspect]
		public Text valueField;


		#endregion Variables
		//uiLabel.text = string.Format("\u20A1 {0:0}", stateData.playerCredits);
	}
}