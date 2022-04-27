using UnityEngine;
using System.Collections;
using SharedCode;
using AdvancedInspector;
using SpaceInsurgency;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class TempEnterSystem : SharedCode.Behaviours.InstanceTracked<TempEnterSystem>
	{
		[Inspect]
		public CanvasGroup canvasGroup;

		[Inspect]
		public Button button;

		
		GalaxyMapDestination m_Focus;
		[Inspect]
		public GalaxyMapDestination Focus
		{
			get { return m_Focus; }
			set { 
				m_Focus = value;

				//Debug.Log("Focus changed");

				canvasGroup.alpha = m_Focus == null ? 0f : 1f;
				canvasGroup.interactable = m_Focus != null;
				canvasGroup.blocksRaycasts = m_Focus != null;
			}
		}

		protected override void Start()
		{
			base.Start();

			Focus = null;
		}

		

		public void EnterCurrentSystem()
		{
			Debug.Log("EnterCurrentSystem()");

			FTLManager.Main.GoToSystem(PlanetarySystem.GetFromUniqueName(Focus.system));
			Focus = null;
		}

		

	}
}