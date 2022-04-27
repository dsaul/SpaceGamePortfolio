using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class ObjectivesCanvas : MonoBehaviour
	{
		[Inspect]
		public GameObject headingRowPrefab;

		[Inspect]
		public GameObject objectiveRowPrefab;

		[Inspect]
		public Sprite checkboxSpriteYes;

		[Inspect]
		public Sprite checkboxSpriteNo;

		[Inspect]
		public Transform rowContainer;

		List<GameObject> rowInstances;
		void Awake()
		{
			rowInstances = new List<GameObject>();
		}

		void Start()
		{
			//RemoveChildren();
			OnFocusChanged(null, QuestDefinition.FocusedQuest);
			QuestDefinition.OnFocusChanged += OnFocusChanged;
			QuestDefinition.OnObjectivesChanged += OnObjectivesChanged;
		}

		void OnFocusChanged(QuestDefinition oldFocus, QuestDefinition newFocus)
		{
			Quest = newFocus;

			RelayUI();
		}

		void OnObjectivesChanged(QuestDefinition quest)
		{
			if (Quest == quest) // only if the quest we are currently showing changed.
			{
				RelayUI();
			}
		}

		QuestDefinition m_Quest;
		[Inspect]
		public QuestDefinition Quest
		{
			get
			{
				return m_Quest;
			}
			set
			{
				m_Quest = value;

				if (false == Application.isPlaying)
					return;

				RelayUI();
			}
		}

		void RelayUI()
		{
			RemoveChildren();

			bool hasQuest = null != m_Quest;
			GetComponent<Animator>().SetBool("visible", hasQuest);

			// Anything that requires a valid m_Quest instance must be below here.
			if (false == hasQuest)
				return;

			// The heading that states the quest name.
			string displayName = m_Quest.DisplayName;

			GameObject headerObject = GameObject.Instantiate<GameObject>(headingRowPrefab);
			rowInstances.Add(headerObject);

			ObjectivesCanvasHeading header = headerObject.GetComponent<ObjectivesCanvasHeading>();
			header.text.text = displayName ?? "(null)";

			headerObject.transform.SetParent(rowContainer, false);

			List<string> objectives = m_Quest.Objectives;
			for (int i=0; i<objectives.Count; i++)
			{
				string objective = objectives[i];

				GameObject objectiveObject = GameObject.Instantiate<GameObject>(objectiveRowPrefab);
				rowInstances.Add(objectiveObject);

				ObjectivesCanvasObjective canvasObjective = objectiveObject.GetComponent<ObjectivesCanvasObjective>();
				canvasObjective.text.text = objective;
				canvasObjective.image.sprite = checkboxSpriteNo;

				objectiveObject.transform.SetParent(rowContainer, false);
			}

			List<string> completedObjectives = m_Quest.CompletedObjectives;
			for (int i = 0; i < completedObjectives.Count; i++)
			{
				string objective = completedObjectives[i];

				GameObject objectiveObject = GameObject.Instantiate<GameObject>(objectiveRowPrefab);
				rowInstances.Add(objectiveObject);

				ObjectivesCanvasObjective canvasObjective = objectiveObject.GetComponent<ObjectivesCanvasObjective>();
				canvasObjective.text.text = objective;
				canvasObjective.image.sprite = checkboxSpriteYes;

				objectiveObject.transform.SetParent(rowContainer, false);
			}
		}

		void RemoveChildren()
		{
			// Remove old rows. We do it this way so we don't have to call DestroyImmediate.
			List<GameObject> children = new List<GameObject>();
			for (int i = 0; i < rowContainer.childCount; i++)
			{
				Transform child = rowContainer.GetChild(i);
				children.Add(child.gameObject);
			}

			for (int i = 0; i < children.Count; i++)
			{
				GameObject child = children[i];
				rowInstances.Remove(child);
				GameObject.Destroy(child);
			}
		}
	}
}