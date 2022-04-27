using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Tooltip
{
	[AdvancedInspector]
	public class Tooltip : SharedCode.Behaviours.InstanceTracked<Tooltip>
	{
		[Inspect, Group("Editor")]
		public Text m_TooltipText;

		string m_Text;

		[Inspect, Group("Runtime")]
		public string Text
		{
			get { return m_Text; }
			set
			{ 
				m_Text = value;

				m_TooltipText.text = m_Text;

				GetComponent<CanvasGroup>().alpha = string.IsNullOrEmpty(m_Text) ? 0f : 1f;
			}
		}


		protected override void Start()
		{
			base.Start();

			Text = null;
		}

		protected override void Update()
		{
			base.Update();
			Vector3 mousePos = Input.mousePosition;
			transform.position = new Vector3(mousePos.x + 20f, mousePos.y - 20f);
		}
	}
}

