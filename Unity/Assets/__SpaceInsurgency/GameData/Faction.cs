using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Faction : SharedCode.Behaviours.InstanceTracked<Faction>
	{
		#region Types / Const

		public enum Relation
		{
			Neutral = 0,
			Amicable = 1,
			Friendly = 2,
			Cautious = 3,
			Hostile = 4
		};
		public const int RelationThresholdFriendly = 50;
		public const int RelationThresholdAmicable = 20;
		public const int RelationThresholdCautious = -20;
		public const int RelationThresholdHostile = -50;

		public const string kFactionPlayer = "kFactionPlayer";
		public const string kFactionImperial = "kFactionImperial";
		public const string kFactionNewIceland = "kFactionNewIceland";
		public const string kFactionNeutral = "kFactionNeutral";
		public const string kFactionRebels = "kFactionRebels";
		public const string kFactionTrader = "kFactionTrader";
		public const string kFactionFreeTradeCoalition = "kFactionFreeTradeCoalition";
		public static readonly List<string> kListOfFactionsForEditor = new List<string> {
			kFactionPlayer,
			kFactionImperial,
			kFactionNewIceland,
			kFactionNeutral,
			kFactionRebels,
			kFactionTrader,
			kFactionFreeTradeCoalition
		};


		#endregion Types / Const
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
		
		[SerializeField]
		string m_DisplayName;
		
		[Inspect]
		public string DisplayName
		{
			get { return m_DisplayName; }
			set { m_DisplayName = value; }
		}

		[SerializeField]
		bool m_IsHardNeutral;

		[Inspect]
		public bool IsHardNeutral
		{
			get { return m_IsHardNeutral; }
			set { m_IsHardNeutral = value; }
		}

		[SerializeField]
		bool m_IsPlayerFaction;

		[Inspect]
		public bool IsPlayerFaction
		{
			get { return m_IsPlayerFaction; }
			set { m_IsPlayerFaction = value; }
		}

		[SerializeField]
		string m_PlayerReputationKey;

		[Inspect]
		public string PlayerReputationKey
		{
			get { return m_PlayerReputationKey; }
			set { m_PlayerReputationKey = value; }
		}

		[SerializeField]
		int m_DefaultReputation;

		[Inspect]
		public int DefaultReputation
		{
			get { return m_DefaultReputation; }
			set { m_DefaultReputation = value; }
		}

		[SerializeField]
		List<Faction> m_GreatRelations;

		[Inspect]
		public List<Faction> GreatRelations
		{
			get { return m_GreatRelations; }
			set { m_GreatRelations = value; }
		}

		[SerializeField]
		List<Faction> m_GoodRelations;

		[Inspect]
		public List<Faction> GoodRelations
		{
			get { return m_GoodRelations; }
			set { m_GoodRelations = value; }
		}

		[SerializeField]
		List<Faction> m_PoorRelations;

		[Inspect]
		public List<Faction> PoorRelations
		{
			get { return m_PoorRelations; }
			set { m_PoorRelations = value; }
		}

		[SerializeField]
		List<Faction> m_TerribleRelations;

		[Inspect]
		public List<Faction> TerribleRelations
		{
			get { return m_TerribleRelations; }
			set { m_TerribleRelations = value; }
		}

		#endregion Variables
		#region Utility Functions

		public static Faction PlayerFaction
		{
			get { return Faction.GetFromUniqueName(kFactionPlayer); }
		}

		public static Faction ImperialFaction
		{
			get { return Faction.GetFromUniqueName(kFactionImperial); }
		}

		public static Faction NewIcelandFaction
		{
			get { return Faction.GetFromUniqueName(kFactionNewIceland); }
		}

		public static Faction NeutralFaction
		{
			get { return Faction.GetFromUniqueName(kFactionNeutral); }
		}

		public static Faction RebelsFaction
		{
			get { return Faction.GetFromUniqueName(kFactionRebels); }
		}

		public static Faction TraderFaction
		{
			get { return Faction.GetFromUniqueName(kFactionTrader); }
		}

		public static Faction FreeTradeCoalitionFaction
		{
			get { return Faction.GetFromUniqueName(kFactionFreeTradeCoalition); }
		}

		[Inspect]
		public Relation DispositionOfPlayer
		{
			get
			{
				if (false == Application.isPlaying)
					return Relation.Neutral;
				return GetFactionDispositionOn(PlayerFaction);
			}
		}

		public Relation GetFactionDispositionOn(Faction otherFaction)
		{
			if (true == otherFaction.IsPlayerFaction)
			{ // This is the player's faction.
				if (true == m_IsHardNeutral)
				{
					return Relation.Neutral;
				}
				else if (PlayerReputation < RelationThresholdHostile)
				{
					return Relation.Hostile;
				}
				else if (PlayerReputation < RelationThresholdCautious)
				{
					return Relation.Cautious;
				}
				else if (PlayerReputation > RelationThresholdFriendly)
				{
					return Relation.Friendly;
				}
				else if (PlayerReputation > RelationThresholdAmicable)
				{
					return Relation.Amicable;
				}
				else
				{
					return Relation.Neutral;
				}
			}
			else
			{ // This is just some faction targetting another faction.

				if (default(Faction) != m_GreatRelations.Find(delegate(Faction o) { return o == otherFaction; }))
				{
					return Relation.Friendly;
				}
				if (default(Faction) != m_GoodRelations.Find(delegate(Faction o) { return o == otherFaction; }))
				{
					return Relation.Amicable;
				}
				if (default(Faction) != m_PoorRelations.Find(delegate(Faction o) { return o == otherFaction; }))
				{
					return Relation.Cautious;
				}
				if (default(Faction) != m_TerribleRelations.Find(delegate(Faction o) { return o == otherFaction; }))
				{
					return Relation.Hostile;
				}

			}
			return Relation.Neutral;
		}

		[Inspect]
		public void CentreCameraOnDynamicAgents()
		{
			DynamicAgent.CentreCameraOnShips(DynamicAgents);
		}

		#endregion Utility Functions
		#region Anccessor

		[Inspect]
		public int PlayerReputation
		{
			get
			{
				if (false == Application.isPlaying)
					return 0;
				if (null == SaveStateManager.Main)
					Debug.LogWarning("null == SaveStateManager.Game");

				return SaveStateManager.First.GetInt(m_PlayerReputationKey, m_DefaultReputation);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				Assert.IsNotNull<SaveStateManager>(SaveStateManager.Main);

				SaveStateManager.Main.SetInt(m_PlayerReputationKey, value);
			}
		}

		[Inspect]
		[ReadOnly]
		public List<DynamicAgent> DynamicAgents
		{
			get
			{
				List<DynamicAgent> ret = new List<DynamicAgent>();
				if (false == Application.isPlaying)
					return ret;
				
				List<Membership> memberships = Membership.Instances;
				for (int i = 0; i < memberships.Count; i++)
				{
					Membership membership = memberships[i];
					DynamicAgent ship = membership.GetComponent<DynamicAgent>();
					if (null != ship && ship.GetComponent<Membership>().Faction == this)
						ret.Add(ship);
				}
				
				return ret;
			}
		}

		public void ClosestShip(Transform reference, out DynamicAgent closestShip, out float closestSqrDistance, DynamicAgent exclude = null)
		{
			closestShip = null;
			closestSqrDistance = float.MaxValue;
			
			List<DynamicAgent> agents = DynamicAgents;
			for (int i=0; i<agents.Count; i++)
			{
				DynamicAgent agent = agents[i];
				if (exclude == agent)
					continue;

				float thisD = reference.position.SqrDistance(agent.transform.position);
				if (thisD < closestSqrDistance)
				{
					closestSqrDistance = thisD;
					closestShip = agent;
				}

			}
		}


		#endregion Anccessor
	}
}