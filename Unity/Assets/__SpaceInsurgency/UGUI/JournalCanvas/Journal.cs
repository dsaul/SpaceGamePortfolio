using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceInsurgency;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency.Journal
{
	[AdvancedInspector]
	public class Journal : SharedCode.Behaviours.InstanceTracked<Journal>
	{

		#region Variables

		[Inspect]
		public GameObject listHeadingPrefab;

		[Inspect]
		public GameObject listItemPrefab;

		[Inspect]
		public GameObject listPlaceholderPrefab;

		[Inspect]
		public GameObject listRowRoot;


		Item m_CurrentSelection;
		[Inspect]
		public Item CurrentSelection
		{
			get { return m_CurrentSelection; }
			set
			{
				if (null != m_CurrentSelection)
					m_CurrentSelection.Selected = false;

				m_CurrentSelection = value;

				QuestDetail.Quest = m_CurrentSelection.Quest;

				if (null != m_CurrentSelection)
					m_CurrentSelection.Selected = true;
			}
		}

		[SerializeField]
		QuestDetail m_QuestDetail;
		[Inspect]
		public QuestDetail QuestDetail
		{
			get { return m_QuestDetail; }
			set { m_QuestDetail = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();

			MakeHidden();

			Relay();

			SubSignal();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubSignal();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnSubSignal();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;

			QuestDefinition.OnFocusChanged += delegate { Relay(); };
			QuestDefinition.OnIsActiveChanged += delegate { Relay(); };
			QuestDefinition.OnIsCompletedChanged += delegate { Relay(); };
			QuestDefinition.OnIsFailedChanged += delegate { Relay(); };

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			
			QuestDefinition.OnFocusChanged -= delegate { Relay(); };
			QuestDefinition.OnIsActiveChanged -= delegate { Relay(); };
			QuestDefinition.OnIsCompletedChanged -= delegate { Relay(); };
			QuestDefinition.OnIsFailedChanged -= delegate { Relay(); };

			signalSub = false;
		}

		#endregion Setup
		#region Events

		[Inspect]
		public void Relay()
		{
			int childCount = listRowRoot.transform.childCount;

			// Remove all children.
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < childCount; i++)
				children.Add(listRowRoot.transform.GetChild(i));
			for (int i=0; i<children.Count; i++)
				GameObject.Destroy(children[i].gameObject);

			GameObject headingActiveObj = GameObject.Instantiate<GameObject>(listHeadingPrefab);
			headingActiveObj.GetComponent<Heading>().text.text = "Active";
			headingActiveObj.transform.SetParent(listRowRoot.transform, false);


			int numAdded = 0;

			using (IEnumerator<QuestDefinition> enumerator = QuestDefinition.ActiveQuests)
				while (enumerator.MoveNext())
				{
					QuestDefinition quest = enumerator.Current;

					GameObject itemObj = GameObject.Instantiate<GameObject>(listItemPrefab);
					itemObj.GetComponent<Item>().text.text = quest.DisplayName;
					itemObj.GetComponent<Item>().Quest = quest;
					itemObj.transform.SetParent(listRowRoot.transform, false);

					numAdded++;
				}

			if (0 == numAdded)
			{
				GameObject placeholderObj = GameObject.Instantiate<GameObject>(listPlaceholderPrefab);
				placeholderObj.transform.SetParent(listRowRoot.transform, false);
			}
			

			GameObject headingCompletedObj = GameObject.Instantiate<GameObject>(listHeadingPrefab);
			headingCompletedObj.GetComponent<Heading>().text.text = "Completed";
			headingCompletedObj.transform.SetParent(listRowRoot.transform, false);

			numAdded = 0;

			using (IEnumerator<QuestDefinition> enumerator = QuestDefinition.CompletedQuests)
				while (enumerator.MoveNext())
				{
					QuestDefinition quest = enumerator.Current;

					GameObject itemObj = GameObject.Instantiate<GameObject>(listItemPrefab);
					itemObj.GetComponent<Item>().text.text = quest.DisplayName;
					itemObj.GetComponent<Item>().Quest = quest;
					itemObj.transform.SetParent(listRowRoot.transform, false);

					numAdded++;
				}

			if (0 == numAdded)
			{
				GameObject placeholderObj = GameObject.Instantiate<GameObject>(listPlaceholderPrefab);
				placeholderObj.transform.SetParent(listRowRoot.transform, false);
			}

			GameObject headingFailedObj = GameObject.Instantiate<GameObject>(listHeadingPrefab);
			headingFailedObj.GetComponent<Heading>().text.text = "Failed";
			headingFailedObj.transform.SetParent(listRowRoot.transform, false);

			numAdded = 0;

			using (IEnumerator<QuestDefinition> enumerator = QuestDefinition.FailedQuests)
				while (enumerator.MoveNext())
				{
					QuestDefinition quest = enumerator.Current;

					GameObject itemObj = GameObject.Instantiate<GameObject>(listItemPrefab);
					itemObj.GetComponent<Item>().text.text = quest.DisplayName;
					itemObj.GetComponent<Item>().Quest = quest;
					itemObj.transform.SetParent(listRowRoot.transform, false);

					numAdded++;
				}

			if (0 == numAdded)
			{
				GameObject placeholderObj = GameObject.Instantiate<GameObject>(listPlaceholderPrefab);
				placeholderObj.transform.SetParent(listRowRoot.transform, false);
			}
		}

		#endregion Events
		#region Public Functions

		public void MakeVisible()
		{
			SpaceInsurgency.Menu.MenuCanvas.First.MakeHidden();
			SpaceInsurgency.Inventory.InventoryCanvas.First.MakeHidden();
			GetComponent<Animator>().SetBool("visible", true);
		}

		public void MakeHidden()
		{
			GetComponent<Animator>().SetBool("visible", false);
		}

		public void SelectQuest(QuestDefinition quest)
		{
			//Debug.Log("SelectQuest "+quest);

			for (int i = 0; i < listRowRoot.transform.childCount; i++)
			{
				Transform child = listRowRoot.transform.GetChild(i);

				Item itemObj = child.GetComponent<Item>();
				//Debug.Log(itemObj);
				if (null == itemObj)
					continue;

				if (itemObj.Quest != quest)
					continue;

				CurrentSelection = itemObj;
				break;
			}



		}





		#endregion Public Functions
	}
}