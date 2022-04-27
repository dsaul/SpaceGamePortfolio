using AdvancedInspector;
using SharedCode;
using SpaceInsurgency.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class SaveStateManager : SharedCode.Behaviours.InstanceTracked<SaveStateManager>
	{
		#region Event Definitions

		public event Action<SaveStateManager> OnBeforeSave;

		#endregion Event Definitions
		#region Variables
		StateData m_SaveState;

		[Inspect]
		public StateData SaveState
		{
			get { return m_SaveState; }
		}

		string m_SaveStateDirectoryPath;


		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();
			
			OnBeforeSave = new Action<SaveStateManager>(delegate { });

			m_SaveStateDirectoryPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dan Saul"), "Space Insurgency");
			Directory.CreateDirectory(m_SaveStateDirectoryPath);

			string lastLoadedPath = PlayerPrefs.GetString("lastLoadedPath", null);

			if (false == File.Exists(lastLoadedPath))
				PlayerPrefs.DeleteKey("lastLoadedPath");
			else
			{
				Debug.Log("SaveStateManager Attempt Load " + lastLoadedPath);
				m_SaveState = SerializeHelper.DeSerializeObject<StateData>(lastLoadedPath);
			}

			if (null == m_SaveState)
			{
				Debug.Log("SaveStateManager New Game ");
				m_SaveState = new StateData();
				PlayerCredits = 0f;
				PlayerFleetLastPlanetarySystem = PlanetarySystem.kPlanetarySystemSol;

				DynamicAgent.Serialized ship1 = new DynamicAgent.Serialized();
				ship1.position = new Vector3(-310f, 0f, -225f);
				ship1.dynamicAgentDefinition = DynamicAgentDefinition.kDynamicAgentDefinitionStealthRecon;
				
				ship1.membership.faction = Faction.kFactionPlayer;

				ship1.sensors.items.Add(Base.kShipModificationSensorRangeIncreaseSmall);

				ship1.weapons.items.Add(Base.kWeaponLaserBolt);

				ship1.cargo.items.Add(Base.kPlayerAvatarLocation);

				ship1.ai.needs.Add(typeof(AINeedDestination).FullName);
				ship1.ai.needs.Add(typeof(AINeedHostility).FullName);

				m_SaveState.dynamicAgents.Add(ship1);


				DynamicAgent.Serialized ship2 = new DynamicAgent.Serialized();
				ship2.position = new Vector3(-311f, 0f, -225f);
				ship2.dynamicAgentDefinition = DynamicAgentDefinition.kDynamicAgentDefinitionStealthRecon;

				ship2.membership.faction = Faction.kFactionPlayer;

				ship2.weapons.items.Add(Base.kWeaponLightBlaster);

				ship2.cargo.items.Add(Base.kCargoIronOre);
				ship2.cargo.items.Add(Base.kCargoIronOre);
				ship2.cargo.items.Add(Base.kCargoIronOre);
				ship2.cargo.items.Add(Base.kCargoIronOre);
				ship2.cargo.items.Add(Base.kCargoIronOre);

				ship2.ai.needs.Add(typeof(AINeedDestination).FullName);
				ship2.ai.needs.Add(typeof(AINeedHostility).FullName);
				m_SaveState.dynamicAgents.Add(ship2);

				Save(ClearFleetFirst: false, SendDelegateBroadcast: false);
			}



		}

		#endregion Setup
		#region Saving

		[Inspect]
		public void SaveFromInspector()
		{
			Save();
		}

		
		public void Save(bool ClearFleetFirst = true, bool SendDelegateBroadcast = true)
		{
			if (true == ClearFleetFirst)
				m_SaveState.dynamicAgents.Clear();
			if (true == SendDelegateBroadcast)
				OnBeforeSave(this);
			
			string filename = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff") + ".xml";
			string combined = Path.Combine(m_SaveStateDirectoryPath, filename);

			Debug.Log("SaveStateManager.Save() " + combined);
			SerializeHelper.SerializeObject(m_SaveState, combined);

			PlayerPrefs.SetString("lastLoadedPath", combined);
		}

		#endregion Saving
		#region Loading

		[Inspect]
		public void QuickLoad()
		{
			LevelGeometry.RemoveAll();
			
			HashSet<Transform> roots = new HashSet<Transform>();
			foreach (Transform transform in UnityEngine.Object.FindObjectsOfType<Transform>())
				roots.Add(transform);

			foreach (Transform transform in roots)
			{
				if (null != transform.GetComponent<LineManager>())
					continue;
				
				GameObject.Destroy(transform.gameObject);
			}

			Application.LoadLevel("_Bootstrap");
			
			//Transform root = transform.root;
			//Debug.Log("root = " + root, root);
		}

		#endregion Loading
		#region Anccessors

		public string PlayerFleetLastPlanetarySystem
		{
			get { return GetString("player.fleet.lastPlanetarySystem", null); }
			set { SetString("player.fleet.lastPlanetarySystem", value); }
		}

		public float PlayerCredits
		{
			get { return GetFloat("player.credits", 0f); }
			set { SetFloat("player.credits", value); }
		}

		public TimeManager.GameTime GameTime
		{
			get { return m_SaveState.gameTime; }
			set { m_SaveState.gameTime = value; }
		}

		#endregion Anccessors
		#region Data Access

		public string GetString(string key, string defaultVal = default(string))
		{
			return m_SaveState.stringStore.GetValueOrDefault<string, string>(key, defaultVal);
		}

		public void SetString(string key, string newval)
		{
			m_SaveState.stringStore[key] = newval;
		}

		public bool GetBool(string key, bool defaultVal = default(bool))
		{
			return m_SaveState.boolStore.GetValueOrDefault<string, bool>(key, defaultVal);
		}

		public void SetBool(string key, bool newval)
		{
			m_SaveState.boolStore[key] = newval;
		}

		public float GetFloat(string key, float defaultVal = default(float))
		{
			return m_SaveState.floatStore.GetValueOrDefault<string, float>(key, defaultVal);
		}

		public void SetFloat(string key, float newval)
		{
			m_SaveState.floatStore[key] = newval;
		}

		public int GetInt(string key, int defaultVal = default(int))
		{
			return m_SaveState.intStore.GetValueOrDefault<string, int>(key, defaultVal);
		}

		public void SetInt(string key, int newval)
		{
			m_SaveState.intStore[key] = newval;
		}

		public Vector3 GetVector3(string key, Vector3 defaultVal = default(Vector3))
		{
			if (false == m_SaveState.floatStore.ContainsKey(key + ".x") &&
				false == m_SaveState.floatStore.ContainsKey(key + ".y") &&
				false == m_SaveState.floatStore.ContainsKey(key + ".z"))
				return defaultVal;

			Vector3 ret = new Vector3();
			ret.x = GetFloat(key + ".x");
			ret.y = GetFloat(key + ".y");
			ret.z = GetFloat(key + ".z");
			return ret;
		}

		public void SetVector3(string key, Vector3 newval)
		{
			SetFloat(key + ".x", newval.x);
			SetFloat(key + ".y", newval.y);
			SetFloat(key + ".z", newval.z);
		}

		public GameObject GetGameObject(string key, GameObject defaultVal = null)
		{
			return m_SaveState.gameObjectStore.GetValueOrDefault<string, GameObject>(key, defaultVal);
		}

		public void SetGameObject(string key, GameObject newval)
		{
			m_SaveState.gameObjectStore[key] = newval;
		}

		#endregion Data Access
		#region Utility

		public void SpawnDynamicAgentsFromSaveState()
		{
			for (int i = 0; i < m_SaveState.dynamicAgents.Count; i++)
				DynamicAgentDefinition.Spawn(m_SaveState.dynamicAgents[i]);
		}

		public IEnumerable<DynamicAgent.Serialized> PlayerSerializedDynamicAgents
		{
			get
			{
				for (int i = 0; i < m_SaveState.dynamicAgents.Count; i++)
					if (m_SaveState.dynamicAgents[i].membership != null && Faction.kFactionPlayer == m_SaveState.dynamicAgents[i].membership.faction)
						yield return m_SaveState.dynamicAgents[i];

				yield break;
			}
		}

		#endregion Utility
	}
}