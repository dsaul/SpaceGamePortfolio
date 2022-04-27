using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using SharedCode;
using System;
using System.Collections.Generic;
using AdvancedInspector;
using System.Linq;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class FTLManager : SharedCode.Behaviours.InstanceTracked<FTLManager>
	{
		#region Event Definitions

		public event Action<FTLManager> OnFTLTransitonBegin; // FTLManager sender
		public event Action<FTLManager> OnFTLTransitonEnd; // FTLManager sender

		#endregion Event Definitions
		#region Variables

		[SerializeField]
		List<AudioClip> m_AudioFTLEntrance;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioFTLEntrance
		{
			get { return m_AudioFTLEntrance; }
			set { m_AudioFTLEntrance = value; }
		}

		[SerializeField]
		List<AudioClip> m_AudioFTLExit;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioFTLExit
		{
			get { return m_AudioFTLExit; }
			set { m_AudioFTLExit = value; }
		}

		[SerializeField]
		GameObject m_EffectFTLInPrefab;
		[Inspect, Group("Effects")]
		public GameObject EffectFTLInPrefab
		{
			get { return m_EffectFTLInPrefab; }
			set { m_EffectFTLInPrefab = value; }
		}

		[SerializeField]
		GameObject m_EffectFTLOutPrefab;
		[Inspect, Group("Effects")]
		public GameObject EffectFTLOutPrefab
		{
			get { return m_EffectFTLOutPrefab; }
			set { m_EffectFTLOutPrefab = value; }
		}

		bool m_TransitionActive = false;
		[Inspect]
		public bool TransitionActive
		{
			get { return m_TransitionActive; }
			set { m_TransitionActive = value; }
		}

		GameObject m_ChosenFTLInPointGameObject;

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			OnFTLTransitonBegin = new Action<FTLManager>(delegate { m_TransitionActive = true; });
			OnFTLTransitonEnd = new Action<FTLManager>(delegate { m_TransitionActive = false; });

			m_ChosenFTLInPointGameObject = new GameObject("m_ChosenFTLInPointGameObject");
			m_ChosenFTLInPointGameObject.transform.parent = transform;
		}

		protected override void Start()
		{
			base.Start();

			SubscribeToEvents();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubscribeToEvents();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnsubscribeToEvents();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnsubscribeToEvents();
		}

		bool eventsSubscribed = false;
		void SubscribeToEvents()
		{
			if (true == eventsSubscribed)
				return;
			
			LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;

			eventsSubscribed = true;
		}

		void UnsubscribeToEvents()
		{
			if (false == eventsSubscribed)
				return;

			LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;
			
			eventsSubscribed = false;
		}

		#endregion Setup
		#region Events

		void LevelGeometry_OnLevelGeometryCreated(LevelGeometry source, bool fromSaveFile)
		{
			
		}

		#endregion Events
		#region Anccessors

		[Inspect, Group("Runtime")]
		public PlanetarySystem CurrentSystem
		{
			get
			{
				if (null == SaveStateManager.Main)
					return null;
				return PlanetarySystem.GetFromUniqueName(SaveStateManager.First.PlayerFleetLastPlanetarySystem);
			}
		}

		#endregion Anccessors
		#region Go To Galaxy Map

		public void GoToGalaxyMap()
		{
			StartCoroutine(_GoToGalaxyMap());
		}

		IEnumerator _GoToGalaxyMap()
		{
			Debug.Log("_GoToGalaxyMap()");

			OnFTLTransitonBegin(this);

			SaveStateManager.First.Save();

			List<DynamicAgent> playerFleetAgents = Faction.PlayerFaction.DynamicAgents;
			for (int i = 0; i < playerFleetAgents.Count; i++)
				playerFleetAgents[i].GetComponent<FTL>().DoFTLOut();

			while (Faction.PlayerFaction.DynamicAgents.Count > 0)
				yield return new WaitForFixedUpdate();

			ScreenFaderCanvas.First.GoToBlack();

			yield return new WaitForSeconds(3f);

			yield return Application.LoadLevelAsync("Galaxy Map 2");

			ScreenFaderCanvas.First.GoToClear();

			OnFTLTransitonEnd(this);

			string lastSystem = SaveStateManager.Main.PlayerFleetLastPlanetarySystem;
			Assert.IsFalse(string.IsNullOrEmpty(lastSystem));

			Debug.Log("lastSystem: " + lastSystem + " " + GalaxyMapDestination.Instances.Count + " " + GalaxyMapDestination.Instances[0].UniqueName + " " + GalaxyMapDestination.Instances[1].UniqueName + " " + GalaxyMapDestination.Instances[2].UniqueName);

			GalaxyMapDestination destination = GalaxyMapDestination.GetFromUniqueName(lastSystem);
			Assert.IsNotNull<GalaxyMapDestination>(destination);



			DynamicAgent.Serialized template = new DynamicAgent.Serialized();
			template.position = destination.transform.position;
			template.dynamicAgentDefinition = DynamicAgentDefinition.kDynamicAgentDefinitionFTLToken;

			template.membership.faction = Faction.kFactionPlayer;

			template.ai.needs.Add(typeof(AINeedDestination).FullName);
			template.ai.needs.Add(typeof(AINeedHostility).FullName);

			DynamicAgentDefinition.Spawn(template);

			//CreatePlayer(true);

			CameraManager.Main.FocusPlayerFleetOrRandomStaticAgent();

			yield break;
		}

		#endregion Go To Galaxy Map
		#region Go To System

		public void GoToSystem(PlanetarySystem _planetarySystem)
		{
			StartCoroutine(_GoToSystem(_planetarySystem));
		}

		IEnumerator _GoToSystem(PlanetarySystem _planetarySystem)
		{
			ScreenFaderCanvas.First.GoToBlack();

			yield return new WaitForSeconds(3f);

			LevelGeometry.RemoveAll();

			// Wait for completion of loading.
			yield return StartCoroutine(LevelGeometry.LoadAsync(_planetarySystem));

			OnFTLTransitonBegin(this);

			SaveStateManager.First.PlayerFleetLastPlanetarySystem = _planetarySystem.UniqueName;

			StaticAgent randomPlanet = StaticAgent.Random;
			//StaticAgent randomPlanet = StaticAgent.GetFromUniqueName("SolEarth"); // for debug

			Vector3 spawnPoint = randomPlanet.GetNewIdleAndSpawnPoint();
			m_ChosenFTLInPointGameObject.transform.position = spawnPoint;
			m_ChosenFTLInPointGameObject.transform.LookAt(randomPlanet.transform);
			//CreatePlayer(true);

			DynamicAgent.Serialized[] savedAgents = SaveStateManager.Main.PlayerSerializedDynamicAgents.ToArray<DynamicAgent.Serialized>();

			List<Vector3> points = FormationSquareish.Get(Const.FormationSpacing, savedAgents.Length);

			
			for (int i = 0; i < savedAgents.Length; i++ )
			{
				savedAgents[i].position = m_ChosenFTLInPointGameObject.transform.TransformPoint(points[i]);

				DynamicAgent dynamicAgent = DynamicAgentDefinition.Spawn(savedAgents[i]);
				dynamicAgent.transform.LookAt(randomPlanet.transform);
				dynamicAgent.GetComponent<FTL>().DoFTLIn();
			}


			// Put the camera somewhere hopefully meaningful.
			CameraManager.Main.FocusPlayerFleetOrRandomStaticAgent();

			ScreenFaderCanvas.First.GoToClear();

			OnFTLTransitonEnd(this);

			yield break;
		}

		#endregion Go To System
	}
}