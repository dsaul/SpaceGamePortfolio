using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SharedCode;

namespace SpaceInsurgency
{
	public class TextToGameTime : SharedCode.Behaviours.Base
	{
		protected override void OnEnable()
		{
			base.OnEnable();

			SetTimer(0.25f, true, OnTimeTickQuarterSecond);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(OnTimeTickQuarterSecond);
		}

		void OnTimeTickQuarterSecond()
		{
			GetComponent<Text>().text = SaveStateManager.First.GameTime.InGameFormattedStringValue;
		}
	}
}