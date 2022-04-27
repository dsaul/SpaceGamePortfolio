using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Bootstrap
{
	[AdvancedInspector]
	public class Bootstrap : SharedCode.Behaviours.Base
	{
		[SerializeField]
		Text m_LoadingText;
		[Inspect]
		public Text LoadingText
		{
			get { return m_LoadingText; }
			set { m_LoadingText = value; }
		}

		protected override void Start()
		{
			base.Start();

			StartCoroutine(LoadGame());
		}

		IEnumerator LoadGame()
		{
			DontDestroyOnLoad(this);
			
			// Wait for application to load.
			yield return new WaitForSeconds(1);

			// Save State Manager
			m_LoadingText.text = ".";
			AsyncOperation op1 = Application.LoadLevelAdditiveAsync("_SaveStateManager");
			yield return op1;

			// Game Data
			m_LoadingText.text = "..";
			AsyncOperation op2 = Application.LoadLevelAdditiveAsync("_GameData");
			yield return op2;

			// Managers
			m_LoadingText.text = "...";
			AsyncOperation op3 = Application.LoadLevelAdditiveAsync("_OtherManagers");
			yield return op3;

			// Managers
			m_LoadingText.text = "....";
			AsyncOperation op4 = Application.LoadLevelAdditiveAsync("_UI");
			yield return op4;

			
			m_LoadingText.text = ".....";
			string saveStatePlanetarySystemUniqueName = SaveStateManager.First.PlayerFleetLastPlanetarySystem;
			Assert.IsFalse(string.IsNullOrEmpty(saveStatePlanetarySystemUniqueName));

			PlanetarySystem saveStatePlanetarySystemObject = PlanetarySystem.GetFromUniqueName(saveStatePlanetarySystemUniqueName);
			Assert.IsNotNull<PlanetarySystem>(saveStatePlanetarySystemObject);

			yield return StartCoroutine(LevelGeometry.LoadAsync(saveStatePlanetarySystemObject, true));

			InputManager.Main.CurrentInputMode = InputManager.InputMode.Game;
			
			// Run the GC at a convenient time.
			
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();

			// Set the default cursor.
			Cursor.SetCursor(InputManager.Main.DefaultCursor, InputManager.Main.DefaultCursorHotspot, CursorMode.Auto);

			// Restore saved dynamic agents.
			SaveStateManager.Main.SpawnDynamicAgentsFromSaveState();

			// Put the camera somewhere hopefully meaningful.
			CameraManager.Main.FocusPlayerFleetOrRandomStaticAgent();

			//Debug.Log("Bootstrap Finished");

			ScreenFaderCanvas.First.GoToClear();
			TopBarCanvas.First.MakeVisible();
			FleetActionsCanvas.First.MakeVisible();
			ActionsCanvas.First.MakeVisible();

			GameObject.Destroy(gameObject);

			yield break;
		}



	}
}