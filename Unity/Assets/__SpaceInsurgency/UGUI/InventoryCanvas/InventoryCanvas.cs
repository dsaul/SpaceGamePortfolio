using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Inventory
{
	[AdvancedInspector]
	public class InventoryCanvas : SharedCode.Behaviours.InstanceTracked<InventoryCanvas>
	{
		#region Variables

		[SerializeField]
		Transform m_DragRootTransform;
		[Inspect, Group("Editor")]
		public Transform DragRootTransform
		{
			get { return m_DragRootTransform; }
			set { m_DragRootTransform = value; }
		}

		[Inspect, Group("Editor")]
		public ShipsContent m_ShipsContent;

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();

			MakeHidden();

			//Relay();

			SubSignal();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubSignal();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;

			signalSub = false;
		}

		#endregion Setup
		#region Events

		#endregion Events
		#region Public Functions

		[Inspect, Group("Utility")]
		public void MakeVisible()
		{
			SpaceInsurgency.Menu.MenuCanvas.First.MakeHidden();
			SpaceInsurgency.Journal.Journal.First.MakeHidden();
			GetComponent<Animator>().SetBool("visible", true);
		}

		[Inspect, Group("Utility")]
		public void MakeHidden()
		{
			GetComponent<Animator>().SetBool("visible", false);
		}

		#endregion Public Functions
	}
}