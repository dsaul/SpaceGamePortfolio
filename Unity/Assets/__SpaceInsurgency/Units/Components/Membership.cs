using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceInsurgency;
using SharedCode;
using System;
using SpaceFaction = SpaceInsurgency.Faction;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Membership : SharedCode.Behaviours.InstanceTracked<Membership>
	{
		#region Variables

		
		[SerializeField]
		string m_InitialFaction;
		
		[Inspect, Group("Editor"), Restrict("ListOfFactionsForEditor")]
		public string InitialFaction
		{
			get { return m_InitialFaction; }
			set { m_InitialFaction = value; }
		}
		public List<string> ListOfFactionsForEditor()
		{
			return Faction.kListOfFactionsForEditor;
		}

		[SerializeField]
		Faction m_Faction;

		[Inspect, Group("Runtime")]
		public Faction Faction
		{
			get
			{
				return m_Faction;
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				Faction oldFaction = m_Faction;
				m_Faction = value;
				OnFactionChanged(this, oldFaction, value);
			}
		}

		#endregion Variables
		#region Event Definitions

		private static Action<Membership, Faction, Faction> _OnFactionChanged;
		public static Action<Membership, Faction, Faction> OnFactionChanged // Membership membership, Faction.Type oldFaction, Faction.Type newFaction
		{
			get
			{
				if (null == _OnFactionChanged)
				{
					_OnFactionChanged = new Action<Membership, Faction, Faction>(delegate { });
				}
				return _OnFactionChanged;
			}
			set
			{
				_OnFactionChanged = value;
			}
		}

		#endregion Event Definitions
		#region Setup

		protected override void Start()
		{
			base.Start();

			if (true == string.IsNullOrEmpty(m_InitialFaction))
				m_InitialFaction = Faction.kFactionNeutral;

			if (null == m_Faction)
				Faction = SpaceInsurgency.Faction.GetFromUniqueName(m_InitialFaction);
		}
		
		#endregion Setup
		#region Ancessors

		[Inspect, Group("Runtime"), ReadOnly]
		public Color DispositionColour
		{
			get
			{
				if (false == Application.isPlaying)
					return Color.magenta;
				if (null == Faction)
					return Color.magenta;

				if (Faction.PlayerFaction == Faction)
					return Color.white;

				switch (Faction.DispositionOfPlayer)
				{
					case SpaceFaction.Relation.Hostile:
						return Color.red;
					case SpaceFaction.Relation.Cautious:
						return Color.yellow;
					case SpaceFaction.Relation.Neutral:
						return Color.white;
					case SpaceFaction.Relation.Amicable:
						return Color.cyan;
					case SpaceFaction.Relation.Friendly:
						return Color.green;
				}
				return Color.white;
			}
		}

		#endregion Ancessors
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public string faction = Faction.kFactionNeutral;
			public string uniqueName = null;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();
			serialized.faction = m_Faction == null ? Faction.kFactionNeutral : m_Faction.UniqueName;
			serialized.uniqueName = UniqueName;
			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			if (false == string.IsNullOrEmpty(serialized.faction))
				Faction = Faction.GetFromUniqueName(serialized.faction);
			else
				Faction = Faction.NeutralFaction;
			UniqueName = serialized.uniqueName;
		}

		#endregion Serialization
	}
}


































