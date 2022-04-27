using UnityEngine;
using System;
using System.Collections;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class Trader : SharedCode.Behaviours.Base
	{
		#region Variables

		[SerializeField]
		bool m_WillTrade = false;

		[Inspect]
		public bool WillTrade
		{
			get { return m_WillTrade; }
			set { m_WillTrade = value; }
		}

		#endregion Variables
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public bool willTrade = false;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();
			serialized.willTrade = WillTrade;
			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			WillTrade = serialized.willTrade;
		}

		#endregion Serialization
	}
}