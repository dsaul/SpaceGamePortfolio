using UnityEngine;
using System.Collections;
using SpaceInsurgency;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency.Menu
{
	[AdvancedInspector]
	public class MenuCanvas : SharedCode.Behaviours.InstanceTracked<MenuCanvas>
	{
		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
			Application.OpenURL("http://google.com");
#else
			Application.Quit();
#endif
		}

		public void OpenControlsEditor()
		{
			cGUI.ToggleGUI();
		}

		public void QuickLoad()
		{
			SaveStateManager.Main.QuickLoad();
		}

#warning have this disabled in the galaxy map, as well as battles.
		public void QuickSave()
		{
			SaveStateManager.Main.Save();
		}

		[Inspect]
		public void MakeVisible()
		{
			SpaceInsurgency.Journal.Journal.First.MakeHidden();
			SpaceInsurgency.Inventory.InventoryCanvas.First.MakeHidden();
			GetComponent<Animator>().SetBool("visible", true);
		}

		[Inspect]
		public void MakeHidden()
		{
			GetComponent<Animator>().SetBool("visible", false);
		}
	}
}