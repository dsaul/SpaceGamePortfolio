using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SharedCode;

namespace SpaceInsurgency
{
	public class TextToPlanetarySystem : SharedCode.Behaviours.Base
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
			if (null == SaveStateManager.First)
				return;

			
			GetComponent<Text>().text = PlanetarySystem.GetFromUniqueName(SaveStateManager.First.PlayerFleetLastPlanetarySystem).DisplayName;
		}
	}
}