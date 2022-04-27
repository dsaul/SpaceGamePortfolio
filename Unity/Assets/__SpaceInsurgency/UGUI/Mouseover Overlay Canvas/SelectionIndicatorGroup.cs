using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;
using SpaceInsurgency;

namespace SpaceInsurgency.Mouseover
{
	[AdvancedInspector]
	public class SelectionIndicatorGroup : SharedCode.Behaviours.Base
	{
		#region Variables

		SpaceObject m_Target;

		[Inspect, ReadOnly, Group("Runtime")]
		public SpaceObject Target
		{
			get { return m_Target; }
			set { m_Target = value; }
		}

		[SerializeField]
		GameObject m_NESprite;

		[Inspect, Group("Editor")]
		public GameObject NESprite
		{
			get { return m_NESprite; }
			set { m_NESprite = value; }
		}

		[SerializeField]
		GameObject m_NWSprite;

		[Inspect, Group("Editor")]
		public GameObject NWSprite
		{
			get { return m_NWSprite; }
			set { m_NWSprite = value; }
		}

		[SerializeField]
		GameObject m_SESprite;

		[Inspect, Group("Editor")]
		public GameObject SESprite
		{
			get { return m_SESprite; }
			set { m_SESprite = value; }
		}

		[SerializeField]
		GameObject m_SWSprite;

		[Inspect, Group("Editor")]
		public GameObject SWSprite
		{
			get { return m_SWSprite; }
			set { m_SWSprite = value; }
		}

		#endregion Variables

		protected override void Update()
		{
			base.Update();

			if (null == m_Target)
				return;
			if (null == m_Target.selectionLastMinX)
				return;
			if (null == m_Target.selectionLastMinY)
				return;
			if (null == m_Target.selectionLastMaxX)
				return;
			if (null == m_Target.selectionLastMaxY)
				return;

			float minX = m_Target.selectionLastMinX.Value;
			float minY = m_Target.selectionLastMinY.Value;
			float maxX = m_Target.selectionLastMaxX.Value;
			float maxY = m_Target.selectionLastMaxY.Value;

			float swX = minX;
			float swY = minY;
			
			float seX = maxX;
			float seY = minY;

			float neX = maxX;
			float neY = maxY;

			float nwX = minX;
			float nwY = maxY;

			m_SWSprite.transform.position = new Vector3(swX, swY, m_SWSprite.transform.position.z);
			m_SESprite.transform.position = new Vector3(seX, seY, m_SESprite.transform.position.z);
			m_NWSprite.transform.position = new Vector3(nwX, nwY, m_NWSprite.transform.position.z);
			m_NESprite.transform.position = new Vector3(neX, neY, m_NESprite.transform.position.z);

			bool targetIsVisible = Target.IsVisibleToCameraGame;

			m_SWSprite.GetComponent<Image>().enabled = targetIsVisible;
			m_SESprite.GetComponent<Image>().enabled = targetIsVisible;
			m_NWSprite.GetComponent<Image>().enabled = targetIsVisible;
			m_NESprite.GetComponent<Image>().enabled = targetIsVisible;

		}


		
	}
}
