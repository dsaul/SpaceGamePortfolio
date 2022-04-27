using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpaceInsurgency;
using AdvancedInspector;

namespace SpaceInsurgency.Journal
{
	[AdvancedInspector]
	public class Item : MonoBehaviour
	{
		[Inspect]
		public Text text;

		[Inspect]
		public Image image;

		[Inspect]
		public Sprite spriteSelected;

		[Inspect]
		public Sprite spriteNotSelected;
		
		bool m_Selected = false;

		[Inspect]
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				m_Selected = value;

				if (Application.isPlaying)
					image.sprite = m_Selected ? spriteSelected : spriteNotSelected;
			}
		}


		QuestDefinition m_Quest;
		[Inspect]
		public QuestDefinition Quest
		{
			get { return m_Quest; }
			set { m_Quest = value; }
		}


		public void RowClicked()
		{
			Journal.First.SelectQuest(Quest);
		}
	}
}