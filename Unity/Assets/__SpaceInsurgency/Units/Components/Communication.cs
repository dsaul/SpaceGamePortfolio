using UnityEngine;
using System;
using System.Collections;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Communication : SharedCode.Behaviours.Base
	{
		#region Variables

		[SerializeField]
		string m_InitialDialogue;

		[Inspect, Group("Editor")]
		public string InitialDialogue
		{
			get { return m_InitialDialogue; }
			set { m_InitialDialogue = value; }
		}


		Dialogue m_Dialogue;
		[Inspect, ReadOnly, Group("Runtime")]
		public Dialogue Dialogue
		{
			get { return m_Dialogue; }
			set { m_Dialogue = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();

			if (null == m_Dialogue && false == string.IsNullOrEmpty(m_InitialDialogue))
				Dialogue = Dialogue.GetFromUniqueName(m_InitialDialogue);
		}

		#endregion Setup
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public string dialogue;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();
			serialized.dialogue = m_Dialogue.UniqueName;
			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			Dialogue = Dialogue.GetFromUniqueName(serialized.dialogue);
		}

		#endregion Serialization
	}
}