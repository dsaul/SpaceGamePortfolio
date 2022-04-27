using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpaceInsurgency;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency.Journal
{
	[AdvancedInspector]
	public class QuestDetail : SharedCode.Behaviours.Base
	{
		#region Variables

		QuestDefinition m_Quest;
		[Inspect]
		public QuestDefinition Quest
		{
			get { return m_Quest; }
			set {
				Debug.Log("Quest detail set quest");
				m_Quest = value;

				Relay();
			}
		}

		[SerializeField]
		Transform m_ListRoot;

		[Inspect]
		public Transform ListRoot
		{
			get { return m_ListRoot; }
			set
			{
				m_ListRoot = value;
			}
		}

		[Inspect]
		public GameObject detailHeadingPrefab;
		[Inspect]
		public GameObject detailRewardPrefab;
		[Inspect]
		public GameObject detailObjectivePrefab;
		[Inspect]
		public GameObject detailDescriptionPrefab;
		[Inspect]
		public GameObject detailButtonsActivePrefab;

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();

			//MakeHidden();

			Relay();

			//SubSignal();
		}

		#endregion Setup
		#region Events
#warning call relay when selected quest is updated
		[Inspect]
		public void Relay()
		{
			int childCount = m_ListRoot.transform.childCount;

			// Remove all children.
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < childCount; i++)
				children.Add(m_ListRoot.transform.GetChild(i));
			for (int i = 0; i < children.Count; i++)
				GameObject.Destroy(children[i].gameObject);

			if (null == m_Quest)
				return;

			GameObject hdrQuestNameObj = GameObject.Instantiate<GameObject>(detailHeadingPrefab);
			hdrQuestNameObj.GetComponent<QuestDetailHeading>().text.text = m_Quest.DisplayName;
			hdrQuestNameObj.transform.SetParent(m_ListRoot.transform, false);

			if (m_Quest.MonetaryReward > 0)
			{
				GameObject rewardActiveObj = GameObject.Instantiate<GameObject>(detailRewardPrefab);
				rewardActiveObj.GetComponent<QuestDetailReward>().valueField.text = string.Format("\u20A1 {0:0}", m_Quest.MonetaryReward);
				rewardActiveObj.transform.SetParent(m_ListRoot.transform, false);
			}

			GameObject hdrObjectives = GameObject.Instantiate<GameObject>(detailHeadingPrefab);
			hdrObjectives.GetComponent<QuestDetailHeading>().text.text = "Objectives:";
			hdrObjectives.transform.SetParent(m_ListRoot.transform, false);

			List<string> objectives = m_Quest.Objectives;
			for (int i = 0; i < objectives.Count; i++)
			{
				GameObject objectiveRowObj = GameObject.Instantiate<GameObject>(detailObjectivePrefab);
				objectiveRowObj.GetComponent<QuestDetailObjectiveRow>().IsCompleted = false;
				objectiveRowObj.GetComponent<QuestDetailObjectiveRow>().objectiveText.text = objectives[i];
				objectiveRowObj.transform.SetParent(m_ListRoot.transform, false);
			}

			List<string> completedObjectives = m_Quest.CompletedObjectives;
			for (int i = 0; i < completedObjectives.Count; i++)
			{
				GameObject objectiveRowObj = GameObject.Instantiate<GameObject>(detailObjectivePrefab);
				objectiveRowObj.GetComponent<QuestDetailObjectiveRow>().IsCompleted = true;
				objectiveRowObj.GetComponent<QuestDetailObjectiveRow>().objectiveText.text = completedObjectives[i];
				objectiveRowObj.transform.SetParent(m_ListRoot.transform, false);
			}

			GameObject description = GameObject.Instantiate<GameObject>(detailDescriptionPrefab);
			description.GetComponent<Text>().text = m_Quest.Description;
			description.transform.SetParent(m_ListRoot.transform, false);

			if (true == m_Quest.IsActive)
			{
				GameObject buttonsActive = GameObject.Instantiate<GameObject>(detailButtonsActivePrefab);
				buttonsActive.GetComponent<QuestDetailButtonsActiveQuest>().abandonButton.onClick.AddListener(ButtonCallbackAbandon);
				buttonsActive.GetComponent<QuestDetailButtonsActiveQuest>().focusButton.onClick.AddListener(ButtonCallbackFocus);
				buttonsActive.transform.SetParent(m_ListRoot.transform, false);
			}

		}

		public void ButtonCallbackFocus()
		{
			Debug.Log("ButtonCallbackFocus()");
#warning complete
		}

		public void ButtonCallbackAbandon()
		{
			Debug.Log("ButtonCallbackAbandon()");
#warning complete
		}

		#endregion Events
	}
}
