using UnityEngine;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class PickupAgent : SharedCode.Behaviours.Base
	{
		[SerializeField]
		GameObject m_SelectionBounds;
		[Inspect]
		public GameObject SelectionBounds
		{
			get { return m_SelectionBounds; }
			set { m_SelectionBounds = value; }
		}

		[SerializeField]
		string m_InitialItem = "";
		[Inspect, Group("Editor")]
		public string InitialItem
		{
			get { return m_InitialItem; }
			set { m_InitialItem = value; }
		}


		Base m_Item;
		[Inspect, ReadOnly, Group("Runtime")]
		public Base Item
		{
			get { return m_Item; }
			set { m_Item = value; }
		}

		protected override void Awake()
		{
			base.Start();

			if (null == m_Item)
			{
				if (false == string.IsNullOrEmpty(m_InitialItem))
					Item = Base.GetFromUniqueName(m_InitialItem);
			}
		}
	}

}