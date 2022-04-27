using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpaceInsurgency;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency.Journal
{
	[AdvancedInspector]
	public class QuestDetailObjectiveRow : MonoBehaviour
	{
		#region Variables

		[Inspect]
		public Sprite completedSprite;
		[Inspect]
		public Sprite notCompletedSprite;

		[Inspect]
		public Image checkImage;
		[Inspect]
		public Text objectiveText;


		bool m_IsCompleted;
		[Inspect]
		public bool IsCompleted
		{
			get { return m_IsCompleted; }
			set
			{
				m_IsCompleted = value;

				checkImage.sprite = m_IsCompleted ? completedSprite : notCompletedSprite;
			}
		}


		#endregion Variables
	}
}
