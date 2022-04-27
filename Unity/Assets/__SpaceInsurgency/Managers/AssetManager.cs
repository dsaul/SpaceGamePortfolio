using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class AssetManager : SharedCode.Behaviours.InstanceTracked<AssetManager>
	{
		[SerializeField]
		GameObject m_ProjectileTrailEnergy;
		[Inspect]
		public GameObject ProjectileTrailEnergy
		{
			get { return m_ProjectileTrailEnergy; }
			set { m_ProjectileTrailEnergy = value; }
		}
		[Inspect]
		public TextAsset rawNamesFemale;
		[Inspect]
		public TextAsset rawNamesMale;
		[Inspect]
		public TextAsset rawNamesLast;
		public string[] NamesFemale;
		public string[] NamesMale;
		public string[] NamesLast;

		protected override void Start()
		{
			base.Start();

			// Create name lists.
			NamesFemale = rawNamesFemale.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			NamesMale = rawNamesMale.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			NamesLast = rawNamesLast.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

		}

	}

}