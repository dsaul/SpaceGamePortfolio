using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class QuestDefinition : SharedCode.Behaviours.InstanceTracked<QuestDefinition>
	{
		#region Types / Const

		#endregion Types / Const
		#region Event Definitions

		static Action<QuestDefinition,bool> _OnIsActiveChanged = null;
		public static Action<QuestDefinition, bool> OnIsActiveChanged // QuestDefinition quest, bool newState
		{
			get
			{
				if (null == _OnIsActiveChanged)
					_OnIsActiveChanged = new Action<QuestDefinition, bool>(delegate(QuestDefinition quest, bool newState) {
						if (true == quest.IsActive && null == FocusedQuest)
							FocusedQuest = quest;
					});
				return _OnIsActiveChanged;
			}
			set
			{
				_OnIsActiveChanged = value;
			}
		}

		static Action<QuestDefinition, bool> _OnIsFailedChanged = null;
		public static Action<QuestDefinition, bool> OnIsFailedChanged // QuestDefinition quest, bool newState
		{
			get
			{
				if (null == _OnIsFailedChanged)
					_OnIsFailedChanged = new Action<QuestDefinition, bool>(delegate(QuestDefinition quest, bool newState) {
						if (true == quest.IsFailed && quest == FocusedQuest)
							FocusedQuest = null;
					});
				return _OnIsFailedChanged;
			}
			set
			{
				_OnIsFailedChanged = value;
			}
		}

		static Action<QuestDefinition,bool> _OnIsCompletedChanged = null;
		public static Action<QuestDefinition, bool> OnIsCompletedChanged // QuestDefinition quest, bool newState
		{
			get
			{
				if (null == _OnIsCompletedChanged)
					_OnIsCompletedChanged = new Action<QuestDefinition, bool>(delegate { });
				return _OnIsCompletedChanged;
			}
			set
			{
				_OnIsCompletedChanged = value;
			}
		}

		static Action<QuestDefinition, QuestDefinition> _OnFocusChanged = null;
		public static Action<QuestDefinition, QuestDefinition> OnFocusChanged // QuestDefinition oldFocus, QuestDefinition newFocus
		{
			get
			{
				if (null == _OnFocusChanged)
					_OnFocusChanged = new Action<QuestDefinition, QuestDefinition>(delegate { });
				return _OnFocusChanged;
			}
			set
			{
				_OnFocusChanged = value;
			}
		}

		static Action<QuestDefinition> _OnObjectivesChanged = null;
		public static Action<QuestDefinition> OnObjectivesChanged // QuestDefinition quest
		{
			get
			{
				if (null == _OnObjectivesChanged)
					_OnObjectivesChanged = new Action<QuestDefinition>(delegate(QuestDefinition quest) { });
				return _OnObjectivesChanged;
			}
			set
			{
				_OnObjectivesChanged = value;
			}
		}

		#endregion Event Definitions
		#region Static Anccessors

		public static IEnumerator<QuestDefinition> ActiveQuests
		{
			get
			{
				List<QuestDefinition> instances = Instances;

				for (int i = 0, count = instances.Count; i < count; i++)
					if (true == instances[i].IsActive)
						yield return instances[i];
			}
		}

		public static IEnumerator<QuestDefinition> FailedQuests
		{
			get
			{
				List<QuestDefinition> instances = Instances;

				for (int i = 0, count = instances.Count; i < count; i++)
					if (true == instances[i].IsFailed)
						yield return instances[i];
			}
		}

		public static IEnumerator<QuestDefinition> CompletedQuests
		{
			get
			{
				List<QuestDefinition> instances = Instances;

				for (int i = 0, count = instances.Count; i < count; i++)
					if (true == instances[i].IsComplete)
						yield return instances[i];
			}
		}

		public static QuestDefinition FocusedQuest
		{
			get
			{
				if (null == SaveStateManager.Main)
					return null;
				return GetFromUniqueName(SaveStateManager.First.SaveState.focusedQuestUniqueName);
			}
			set
			{
				QuestDefinition old = GetFromUniqueName(SaveStateManager.First.SaveState.focusedQuestUniqueName);
				
				if (null == value)
				{
					SaveStateManager.First.SaveState.focusedQuestUniqueName = null;
					OnFocusChanged(old, null);
				}
				else
				{
					SaveStateManager.First.SaveState.focusedQuestUniqueName = value.UniqueName;
					OnFocusChanged(old, value);
				}
			}
		}

		#endregion Static Anccessors
		#region Variables

		[Inspect]
		public override string UniqueName
		{
			get
			{
				return base.UniqueName;
			}
			set
			{
				base.UniqueName = value;
			}
		}

		[Inspect, Group("Runtime")]
		public virtual string DisplayName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("DisplayName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("DisplayName",value);
			}
		}

		[Inspect, Group("Runtime")]
		public virtual bool QuestAvailable
		{
			get 
			{
				if (false == Application.isPlaying)
					return false;
				return GetBool("QuestAvailable", false); 
			}
			set 
			{
				if (false == Application.isPlaying)
					return;
				SetBool("QuestAvailable", value); 
			}
		}

		[Inspect, Group("Runtime")]
		public virtual string Description
		{
			get 
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("Description", null); 
			}
			set 
			{
				if (false == Application.isPlaying)
					return;
				SetString("Description", value); 
			}
		}

		[SerializeField]
		List<string> m_Objectives;

		[Inspect, Group("Runtime")]
		public virtual List<string> Objectives
		{
			get { return m_Objectives; }
			set { m_Objectives = value; }
		}

		[SerializeField]
		List<string> m_CompletedObjectives;

		[Inspect, Group("Runtime")]
		public virtual List<string> CompletedObjectives
		{
			get { return m_CompletedObjectives; }
			set { m_CompletedObjectives = value; }
		}

		[Inspect, Group("Runtime")]
		public virtual float MonetaryReward
		{
			get 
			{
				if (false == Application.isPlaying)
					return 0f;
				return GetFloat("MonetaryReward", 0f); 
			}
			set 
			{
				if (false == Application.isPlaying)
					return;
				SetFloat("MonetaryReward", value); 
			}
		}

		[Inspect, Group("Runtime")]
		public bool IsActive
		{
			get 
			{
				if (false == Application.isPlaying)
					return false;
				return GetBool("IsActive", false); 
			}
			set 
			{
				if (false == Application.isPlaying)
					return;
				SetBool("IsActive", value);
				OnIsActiveChanged(this,value);
			}
		}

		[Inspect, Group("Runtime")]
		public bool IsComplete
		{
			get 
			{
				if (false == Application.isPlaying)
					return false;
				return GetBool("IsComplete", false);
			}
			set 
			{
				if (false == Application.isPlaying)
					return;
				SetBool("IsComplete", value);

				if (true == value)
				{
					IsActive = false;
					FocusedQuest = null;
				}

				OnIsCompletedChanged(this, value);
				
				
			}
		}

		[Inspect, Group("Runtime")]
		public virtual bool IsFailed
		{
			get 
			{
				if (false == Application.isPlaying)
					return false;
				return GetBool("IsFailed", false); 
			}
			set 
			{
				if (false == Application.isPlaying)
					return;
				SetBool("IsFailed", value);
				OnIsFailedChanged(this,value);
			}
		}

		[Inspect, Group("Runtime")]
		public virtual bool MeetsCompleteConditions
		{
			get
			{
				return false;
			}
		}

		#endregion Variables
		#region Utility

		protected string GetString(string keypart, string defaultVal)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			return SaveStateManager.First.GetString(key, defaultVal);
		}

		protected void SetString(string keypart, string newval)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			SaveStateManager.First.SetString(key, newval);
		}

		protected bool GetBool(string keypart, bool defaultVal)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			return SaveStateManager.First.GetBool(key, defaultVal);
		}

		protected void SetBool(string keypart, bool newval)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			SaveStateManager.First.SetBool(key, newval);
		}

		protected float GetFloat(string keypart, float defaultVal)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			return SaveStateManager.First.GetFloat(key, defaultVal);
		}

		protected void SetFloat(string keypart, float newval)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			SaveStateManager.First.SetFloat(key, newval);
		}

		protected int GetInt(string keypart, int defaultVal)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			return SaveStateManager.First.GetInt(key, defaultVal);
		}

		protected void SetInt(string keypart, int newval)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			SaveStateManager.First.SetInt(key, newval);
		}

		protected GameObject GetGameObject(string keypart, GameObject defaultVal)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			return SaveStateManager.First.GetGameObject(key, defaultVal);
		}

		protected void SetGameObject(string keypart, GameObject newval)
		{
			string key = string.Format("quest.{0}.{1}", UniqueName, keypart);
			SaveStateManager.First.SetGameObject(key, newval);
		}

		[Inspect, Group("Runtime")]
		public void ActivateQuest()
		{
			IsActive = true;
		}

		[Inspect, Group("Runtime")]
		public virtual void ResetQuest()
		{
			IsActive = false;
			IsComplete = false;
			IsFailed = false;
		}

		

		#endregion Utility
	}
}