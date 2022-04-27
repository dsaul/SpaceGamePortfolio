using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	public class MessageLogCanvas : SharedCode.Behaviours.InstanceTracked<MessageLogCanvas>
	{
		[Serializable]
		public class Message
		{
			public string Text { get; set; }
			public float GameTimeRecieved { get; set; }
			public GameObject VisibleObject { get; set; }
		}

		List<Message> m_Messages;
		[Inspect]
		public List<Message> Messages
		{
			get { return m_Messages; }
		}

		[SerializeField]
		int m_MaxMessages = 4;
		[Inspect]
		public int MaxMessages
		{
			get { return m_MaxMessages; }
			set { m_MaxMessages = value; }
		}

		[SerializeField]
		float m_MaxMessageDuration = 5;
		[Inspect]
		public float MaxMessageDuration
		{
			get { return m_MaxMessageDuration; }
			set { m_MaxMessageDuration = value; }
		}

		[Inspect]
		public GameObject rowPrefab;

		[Inspect]
		public GameObject rowRoot;

		protected override void Awake()
		{
			base.Awake();

			m_Messages = new List<Message>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SetTimer(0.25f, true, RemoveExpiredMessages);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(RemoveExpiredMessages);
		}

		public void AddMessage(string text)
		{
			// If we are at the max message count, remove the oldest message.
			Message oldest = null;
			if (MaxMessages == m_Messages.Count)
			{
				for (int i=0; i<m_Messages.Count; i++)
				{
					if (null == oldest)
						oldest = m_Messages[i];
					else if (m_Messages[i].GameTimeRecieved < oldest.GameTimeRecieved)
						oldest = m_Messages[i];
				}
				if (null != oldest)
				{
					GameObject.Destroy(oldest.VisibleObject);
					m_Messages.Remove(oldest);
				}
			}
			
			
			Message msg = new Message { Text = text, GameTimeRecieved = Time.time };

			GameObject obj = GameObject.Instantiate<GameObject>(rowPrefab);
			obj.transform.SetParent(rowRoot.transform, false);

			Text t = obj.GetComponent<Text>();
			t.text = text;

			msg.VisibleObject = obj;
			m_Messages.Add(msg);

			UpdateStateMachine();

			//Debug.Log("AddMessage " + text);
		}

		void RemoveExpiredMessages()
		{
			int i = m_Messages.Count;
			while (--i >= 0)
			{
				if ((m_Messages[i].GameTimeRecieved + MaxMessageDuration) < Time.time)
				{
					//GameObject.Destroy(m_Messages[i].VisibleObject);
					m_Messages[i].VisibleObject.GetComponent<Animator>().SetBool("StartRemoving", true);
					m_Messages.RemoveAt(i);

					UpdateStateMachine();
				}
			}
		}

		void UpdateStateMachine()
		{
			GetComponent<Animator>().SetInteger("MessageCount", m_Messages.Count);
		}
	}
}
