using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class DynamicAgentDefinition : SharedCode.Behaviours.InstanceTracked<DynamicAgentDefinition>
	{
		#region Types / Const

		public const string kDynamicAgentDefinitionBomber = "kDynamicAgentDefinitionBomber";
		public const string kDynamicAgentDefinitionCommandShip = "kDynamicAgentDefinitionCommandShip";
		public const string kDynamicAgentDefinitionDropship = "kDynamicAgentDefinitionDropship";
		public const string kDynamicAgentDefinitionFighter = "kDynamicAgentDefinitionFighter";
		public const string kDynamicAgentDefinitionFuelTransport = "kDynamicAgentDefinitionFuelTransport";
		public const string kDynamicAgentDefinitionGasHarvester = "kDynamicAgentDefinitionGasHarvester";
		public const string kDynamicAgentDefinitionHeavyFighter = "kDynamicAgentDefinitionHeavyFighter";
		public const string kDynamicAgentDefinitionInterceptor = "kDynamicAgentDefinitionInterceptor";
		public const string kDynamicAgentDefinitionMiningShip = "kDynamicAgentDefinitionMiningShip";
		public const string kDynamicAgentDefinitionPirate = "kDynamicAgentDefinitionPirate";
		public const string kDynamicAgentDefinitionRepairShip = "kDynamicAgentDefinitionRepairShip";
		public const string kDynamicAgentDefinitionSmallTransport = "kDynamicAgentDefinitionSmallTransport";
		public const string kDynamicAgentDefinitionStealthRecon = "kDynamicAgentDefinitionStealthRecon";
		public const string kDynamicAgentDefinitionSuperiorityFighter = "kDynamicAgentDefinitionSuperiorityFighter";
		public const string kDynamicAgentDefinitionTransport = "kDynamicAgentDefinitionTransport";
		public const string kDynamicAgentDefinitionSensorBoey = "kDynamicAgentDefinitionSensorBoey";
		public const string kDynamicAgentDefinitionFTLToken = "kDynamicAgentDefinitionFTLToken";
		public static readonly List<string> kListOfDynamicAgentDefinitionsForEditor = new List<string> {
			kDynamicAgentDefinitionBomber,
			kDynamicAgentDefinitionCommandShip,
			kDynamicAgentDefinitionDropship,
			kDynamicAgentDefinitionFighter,
			kDynamicAgentDefinitionFuelTransport,
			kDynamicAgentDefinitionGasHarvester,
			kDynamicAgentDefinitionHeavyFighter,
			kDynamicAgentDefinitionInterceptor,
			kDynamicAgentDefinitionMiningShip,
			kDynamicAgentDefinitionPirate,
			kDynamicAgentDefinitionRepairShip,
			kDynamicAgentDefinitionSmallTransport,
			kDynamicAgentDefinitionStealthRecon,
			kDynamicAgentDefinitionSuperiorityFighter,
			kDynamicAgentDefinitionTransport,
			kDynamicAgentDefinitionSensorBoey,
			kDynamicAgentDefinitionFTLToken
		};


		#endregion Types / Const
		#region Variables

		[Inspect]
		public override string UniqueName
		{
			get
			{
				return base.UniqueName;
			}
			set
			{
				base.UniqueName = value;
			}
		}

		[SerializeField]
		GameObject m_BodyPrefab;
		[Inspect, Group("Prefabs"), DontAllowSceneObject]
		public GameObject BodyPrefab
		{
			get { return m_BodyPrefab; }
			set { m_BodyPrefab = value; }
		}

		AutoObjectPool<GameObject, DynamicAgentDefinition> m_BodyPrefabPool;
		public AutoObjectPool<GameObject, DynamicAgentDefinition> BodyPrefabPool
		{
			get { return m_BodyPrefabPool; }
		}

		[SerializeField]
		float m_DefaultSensorRange;
		[Inspect, Group("Agent Stats")]
		public float DefaultSensorRange
		{
			get { return m_DefaultSensorRange; }
			set { m_DefaultSensorRange = value; }
		}

		[SerializeField]
		List<AudioClip> m_AudioOnSelect;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioOnSelect
		{
			get { return m_AudioOnSelect; }
			set { m_AudioOnSelect = value; }
		}

		[SerializeField]
		List<AudioClip> m_AudioOnBreakup;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioOnBreakup
		{
			get { return m_AudioOnBreakup; }
			set { m_AudioOnBreakup = value; }
		}

		[SerializeField]
		List<AudioClip> m_AudioOnExplode;
		[Inspect, Group("Audio")]
		public List<AudioClip> AudioOnExplode
		{
			get { return m_AudioOnExplode; }
			set { m_AudioOnExplode = value; }
		}

		[SerializeField]
		string m_StoreDescription = "";
		[Inspect, Group("Information")]
		public string StoreDescription
		{
			get { return m_StoreDescription; }
			set { m_StoreDescription = value; }
		}

		[SerializeField]
		string m_DisplayName;
		[Inspect, Group("Information")]
		public string DisplayName
		{
			get { return m_DisplayName; }
			set { m_DisplayName = value; }
		}

		[SerializeField]
		string m_ShortDisplayName;
		[Inspect, Group("Information")]
		public string ShortDisplayName
		{
			get { return m_ShortDisplayName; }
			set { m_ShortDisplayName = value; }
		}

		[SerializeField]
		float m_HealthMaxBase = 100f;
		[Inspect, Group("Agent Stats")]
		public float HealthMaxBase
		{
			get { return m_HealthMaxBase; }
			set { m_HealthMaxBase = value; }
		}

		[SerializeField]
		float m_HealthRechargeBase = 0f;
		[Inspect, Group("Agent Stats")]
		public float HealthRechargeBase
		{
			get { return m_HealthRechargeBase; }
			set { m_HealthRechargeBase = value; }
		}

		[SerializeField]
		float m_EnergyMaxBase = 100f;
		[Inspect, Group("Agent Stats")]
		public float EnergyMaxBase
		{
			get { return m_EnergyMaxBase; }
			set { m_EnergyMaxBase = value; }
		}

		[SerializeField]
		float m_EnergyRechargeBase = 5f;
		[Inspect, Group("Agent Stats")]
		public float EnergyRechargeBase
		{
			get { return m_EnergyRechargeBase; }
			set { m_EnergyRechargeBase = value; }
		}

		[SerializeField]
		float m_ShieldMaxBase = 100f;
		[Inspect, Group("Agent Stats")]
		public float ShieldMaxBase
		{
			get { return m_ShieldMaxBase; }
			set { m_ShieldMaxBase = value; }
		}

		[SerializeField]
		float m_ShieldRechargeBase = 2f;
		[Inspect, Group("Agent Stats")]
		public float ShieldRechargeBase
		{
			get { return m_ShieldRechargeBase; }
			set { m_ShieldRechargeBase = value; }
		}

		[SerializeField]
		float m_WeightMaxBase = 100f;
		[Inspect, Group("Agent Stats")]
		public float WeightMaxBase
		{
			get { return m_WeightMaxBase; }
			set { m_WeightMaxBase = value; }
		}

		[SerializeField]
		int m_WeaponSlots = 2;

		[Inspect, Group("Components")]
		public int WeaponSlots
		{
			get { return m_WeaponSlots; }
			set { m_WeaponSlots = value; }
		}

		[SerializeField]
		float m_MoveSpeedBase = 10f;
		[Inspect, Group("Agent Stats")]
		public float MoveSpeedBase
		{
			get { return m_MoveSpeedBase; }
			set { m_MoveSpeedBase = value; }
		}

		[SerializeField]
		float m_CostBase = 1000f;
		[Inspect, Group("Information")]
		public float CostBase
		{
			get { return m_CostBase; }
			set { m_CostBase = value; }
		}

		[SerializeField]
		GameObject m_ExplosionEffectPrefab;
		[Inspect, Group("Prefabs"), DontAllowSceneObject]
		public GameObject ExplosionEffectPrefab
		{
			get { return m_ExplosionEffectPrefab; }
			set { m_ExplosionEffectPrefab = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();

			m_BodyPrefabPool = new AutoObjectPool<GameObject, DynamicAgentDefinition>(
				delegate(DynamicAgentDefinition pfbType) {
					GameObject obj = GameObject.Instantiate(pfbType.BodyPrefab) as GameObject;
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;
					return obj;
				},
				delegate(GameObject obj) {
					obj.SetActive(false);
					obj.transform.parent = transform;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localScale = Vector3.one;
					obj.GetComponent<ShipModelParts>().Ship = null;
				},
				20
			);
			m_BodyPrefabPool.PreloadObjects(this);
		}

		#endregion Setup
		#region Spawning

		public static DynamicAgent Spawn(DynamicAgent.Serialized serialized)
		{
			DynamicAgentDefinition dynamicAgentDefinition = DynamicAgentDefinition.GetFromUniqueName(serialized.dynamicAgentDefinition);
			Assert.IsNotNull<DynamicAgentDefinition>(dynamicAgentDefinition);
			
			// Actually create the ship.
			GameObject dynamicAgentObject = new GameObject("Dynamic Agent Base");
			dynamicAgentObject.SetActive(false);

			DynamicAgent dynamicAgent = dynamicAgentObject.AddComponent<DynamicAgent>();
			Assert.IsNotNull<DynamicAgent>(dynamicAgent);
			
			dynamicAgent.RestoreSerialized(serialized);

			dynamicAgentObject.name = "ship";
			
			dynamicAgentObject.SetActive(true);

			
			
			// Add visual object.
			dynamicAgent.VisualObject3D = dynamicAgentDefinition.BodyPrefabPool.Get(dynamicAgentDefinition);
			dynamicAgent.VisualObject3D.transform.parent = dynamicAgent.AnimationContainer.transform;
			dynamicAgent.VisualObject3D.transform.localPosition = Vector3.zero;
			dynamicAgent.VisualObject3D.transform.localRotation = Quaternion.identity;

			dynamicAgent.Parts = dynamicAgent.VisualObject3D.GetComponent<ShipModelParts>();
			dynamicAgent.Parts.Ship = dynamicAgent;

			dynamicAgent.ShipShieldObject = dynamicAgent.Parts.shield;

			dynamicAgent.VisualObject3D.SetActive(true);

			// Notify done.
			dynamicAgent.OnFinishedSpawnSetup(dynamicAgent);
			
			return dynamicAgent;
		}
		
		#endregion Spawning
		#region Utility

		[Inspect, Group("Utility")]
		public void DebugSpawnFriendlyShip()
		{
			DynamicAgent.Serialized serialized = new DynamicAgent.Serialized();
			serialized.dynamicAgentDefinition = UniqueName;
			serialized.position = InputManager.Main.MouseMovementPlaneLocationFudged;
			serialized.rotation = Quaternion.identity;

			serialized.membership.faction = Faction.kFactionImperial;

			serialized.weapons.items.Add(Base.kWeaponVulcan);

			serialized.ai.needs.Add(typeof(AINeedDestination).FullName);
			serialized.ai.needs.Add(typeof(AINeedHostility).FullName);

			DynamicAgent ship = DynamicAgentDefinition.Spawn(serialized);
			
			ship.GetComponent<FTL>().DoFTLIn();
			
			
			
			
		}

		[Inspect, Group("Utility")]
		public void DebugSpawnHostileShip()
		{
			DynamicAgent.Serialized serialized = new DynamicAgent.Serialized();
			serialized.dynamicAgentDefinition = UniqueName;
			serialized.position = InputManager.Main.MouseMovementPlaneLocationFudged;
			serialized.rotation = Quaternion.identity;

			serialized.membership.faction = Faction.kFactionFreeTradeCoalition;

			serialized.weapons.items.Add(Base.kWeaponVulcan);

			serialized.ai.needs.Add(typeof(AINeedDestination).FullName);
			serialized.ai.needs.Add(typeof(AINeedHostility).FullName);

			DynamicAgent ship = DynamicAgentDefinition.Spawn(serialized);
			
			ship.GetComponent<FTL>().DoFTLIn();
		}

		#endregion Utility
	}
}