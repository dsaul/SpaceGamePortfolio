using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class FleetActionsCanvas : SharedCode.Behaviours.InstanceTracked<FleetActionsCanvas>
	{
		#region Variables

		[Inspect]
		public Sprite fleetPanelCommunicationSprite;

		[Inspect]
		public Sprite fleetPanelGalaxyMapSprite;

		[Inspect]
		public GameObject itemPrefab;

		[Inspect]
		public Transform itemRoot;

		#endregion Variables

		

		protected override void Start()
		{
			base.Start();

			MakeHidden();

			FleetActionsCanvas.First.AddItem(fleetPanelGalaxyMapSprite, delegate { FTLManager.Main.GoToGalaxyMap(); }, delegate {
				if (null == LevelGeometry.Main)
					return false;
				return false == LevelGeometry.Main.IsGalaxyMap; 
			});
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SetTimer(0.25f, true, CheckEnableTimer);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(CheckEnableTimer);
		}

		void CheckEnableTimer()
		{
			for (int i=0; i<FleetActionsCanvasItem.Instances.Count; i++)
				FleetActionsCanvasItem.Instances[i].CheckInteractableNow();
		}

		public void MakeVisible()
		{
			GetComponent<Animator>().SetBool("visible", true);
		}

		public void MakeHidden()
		{
			GetComponent<Animator>().SetBool("visible", false);
		}

		public void AddItem(Sprite Sprite, Action OnClickAction, Func<bool> CheckEnableFunc)
		{
			GameObject obj = GameObject.Instantiate<GameObject>(itemPrefab);
			obj.transform.SetParent(itemRoot, false);

			FleetActionsCanvasItem item = obj.GetComponent<FleetActionsCanvasItem>();
			item.OnClickAction = OnClickAction;
			item.CheckEnableFunc = CheckEnableFunc;

			Image img = obj.GetComponent<Image>();
			img.sprite = Sprite;
		}
	}
}