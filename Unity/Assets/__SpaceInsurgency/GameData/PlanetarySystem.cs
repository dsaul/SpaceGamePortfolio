using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class PlanetarySystem : SharedCode.Behaviours.InstanceTracked<PlanetarySystem>
	{
		#region Types / Const

		public const string kPlanetarySystemSol = "kPlanetarySystemSol";
		public const string kPlanetarySystemTauCeti = "kPlanetarySystemTauCeti";
		public const string kPlanetarySystemWolf359 = "kPlanetarySystemWolf359";
		public static readonly List<string> kListOfPlanetarySystemsForEditor = new List<string> {
			kPlanetarySystemSol,
			kPlanetarySystemTauCeti,
			kPlanetarySystemWolf359
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
		string m_SceneName;

		[Inspect]
		public string SceneName
		{
			get { return m_SceneName; }
			set { m_SceneName = value; }
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
		Faction m_HomeFaction;

		[Inspect]
		public Faction HomeFaction
		{
			get { return m_HomeFaction; }
			set { m_HomeFaction = value; }
		}

		#endregion Variables
		
	}
}