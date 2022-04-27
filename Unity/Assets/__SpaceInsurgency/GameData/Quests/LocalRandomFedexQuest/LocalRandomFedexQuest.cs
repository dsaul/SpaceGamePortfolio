using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class LocalRandomFedexQuest : QuestDefinition
	{
		#region Variables

		List<StaticAgent> m_PlanetsInThisSystem;

		[Inspect, Group("Runtime")]
		public string LastUpdatedSystemUniqueName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("LastUpdatedSystemUniqueName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("LastUpdatedSystemUniqueName", value);
			}
		}

		[Inspect, Group("Runtime")]
		public string DestinationSystemUniqueName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("DestinationSystemUniqueName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("DestinationSystemUniqueName", value);
			}
		}

		[Inspect, Group("Runtime")]
		public string SourcePlanetUniqueName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("SourcePlanetUniqueName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("SourcePlanetUniqueName", value);
			}
		}

		[Inspect, Group("Runtime")]
		public string SourcePlanetDisplayName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("SourcePlanetDisplayName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("SourcePlanetDisplayName", value);
			}
		}

		[Inspect, Group("Runtime")]
		public string DestinationPlanetUniqueName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("DestinationPlanetUniqueName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("DestinationPlanetUniqueName", value);
			}
		}

		[Inspect, Group("Runtime")]
		public string DestinationPlanetDisplayName
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetString("DestinationPlanetDisplayName", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetString("DestinationPlanetDisplayName", value);
			}
		}

		[Inspect, Group("Runtime")]
		public bool HasPickedUpMessage
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				return GetBool("HasPickedUpMessage", false);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetBool("HasPickedUpMessage", value);

				UpdateObjectives();
				QuestDefinition.OnObjectivesChanged(this);
			}
		}

		[Inspect, Group("Runtime")]
		public bool HasDeliveredMessage
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				return GetBool("HasDeliveredMessage", false);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetBool("HasDeliveredMessage", value);

				UpdateObjectives();
				QuestDefinition.OnObjectivesChanged(this);
			}
		}

		[Inspect, Group("Runtime")]
		public GameObject MessageBoeyGameObject
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				return GetGameObject("MessageBoeyGameObject", null);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetGameObject("MessageBoeyGameObject", value);
			}
		}

		[Inspect, Group("Editor")]
		public float PickupLaunchSqrDistance
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return GetFloat("PickupLaunchSqrDistance", 60f);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetFloat("PickupLaunchSqrDistance", value);
			}
		}

		[Inspect, Group("Editor")]
		public float DeliverLaunchSqrDistance
		{
			get
			{
				if (false == Application.isPlaying)
					return 0f;
				return GetFloat("DeliverLaunchSqrDistance", 60f);
			}
			set
			{
				if (false == Application.isPlaying)
					return;
				SetFloat("DeliverLaunchSqrDistance", value);
			}
		}

		[SerializeField]
		GameObject m_MessageBoeyPrefab;
		[Inspect, Group("Editor"), DontAllowSceneObject]
		public GameObject MessageBoeyPrefab
		{
			get { return m_MessageBoeyPrefab; }
			set { m_MessageBoeyPrefab = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			m_PlanetsInThisSystem = new List<StaticAgent>();
		}

		protected override void Start()
		{
			base.Start();

			SubSignal();
			LevelGeometry_OnLevelGeometryCreated(null, false);

			SetTimer(1f, true, CheckCloseToSource);
			SetTimer(1f, true, CheckCloseToDestination);
			SetTimer(1f, true, CheckComplete);
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

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnSubSignal();

			ClearTimer(CheckCloseToSource);
			ClearTimer(CheckCloseToDestination);
			ClearTimer(CheckComplete);
		}

		bool signalSubed = false;

		void SubSignal()
		{
			if (false == signalSubed)
			{
				if (null == FTLManager.Main)
					return;
				LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;
				signalSubed = true;
			}
		}

		void UnSubSignal()
		{
			if (true == signalSubed)
			{
				if (null == FTLManager.Main)
					return;
				LevelGeometry.OnLevelGeometryCreated += LevelGeometry_OnLevelGeometryCreated;
				signalSubed = false;
			}
		}

		public override void ResetQuest()
		{
			base.ResetQuest();

			LastUpdatedSystemUniqueName = null;
			DestinationSystemUniqueName = null;
			SourcePlanetUniqueName = null;
			SourcePlanetDisplayName = null;
			DestinationPlanetUniqueName = null;
			DestinationPlanetDisplayName = null;
			HasPickedUpMessage = false;
			HasDeliveredMessage = false;
			MessageBoeyGameObject = null;
			DisplayName = null;
			Description = null;

			// Generate a new quest.
			LevelGeometry_OnLevelGeometryCreated(null, false);
		}

		#endregion Setup
		#region Events



		void LevelGeometry_OnLevelGeometryCreated(LevelGeometry source, bool fromSaveFile)
		{
			if (null == FTLManager.Main)
				return;
			
			// Make sure we have a system loaded.
			PlanetarySystem currentSystem = FTLManager.Main.CurrentSystem;
			if (null == currentSystem)
				return;

			// We only want to update the quest if it is a new system.
			if (false == string.IsNullOrEmpty(LastUpdatedSystemUniqueName))
				if (currentSystem.UniqueName == LastUpdatedSystemUniqueName)
					return;

			// We don't want to update this while it is active.
			if (true == IsActive)
				return;

			DestinationSystemUniqueName = currentSystem.UniqueName;

			m_PlanetsInThisSystem.Clear();

			List<StaticAgent> planets = StaticAgent.Instances;
			for (int i = 0; i < planets.Count; i++)
			{
				StaticAgent planet = planets[i];
				if (false == planet.AllowAsLocalMessageDeliveryDestination)
					continue;

				m_PlanetsInThisSystem.Add(planet);
			}

			// We need at least elligible two planets for this quest to work.
			if (m_PlanetsInThisSystem.Count < 2)
				return;

			// Choose a random one.
			StaticAgent sourcePlanet = m_PlanetsInThisSystem.RandomItem<StaticAgent>();
			m_PlanetsInThisSystem.Remove(sourcePlanet);
			SourcePlanetUniqueName = sourcePlanet.UniqueName;
			SourcePlanetDisplayName = sourcePlanet.DisplayName;

			StaticAgent destinationPlanet = m_PlanetsInThisSystem.RandomItem<StaticAgent>();
			m_PlanetsInThisSystem.Remove(destinationPlanet);
			DestinationPlanetUniqueName = destinationPlanet.UniqueName;
			DestinationPlanetDisplayName = destinationPlanet.DisplayName;

#warning add a % chance
			QuestAvailable = true;
			DisplayName = string.Format("Local Random Fedex Quest {0} to {1}", sourcePlanet.DisplayName, destinationPlanet.DisplayName);
			Description = string.Format("Step 1: Move to {0}. \nStep 2: Move to {1}. \nStep 3: ????? \nStep 4: Profit!", sourcePlanet.DisplayName, destinationPlanet.DisplayName);
			MonetaryReward = UnityEngine.Random.Range(100, 250);
			UpdateObjectives();

			LastUpdatedSystemUniqueName = currentSystem.UniqueName;
		}

		void UpdateObjectives()
		{
			Objectives.Clear();
			CompletedObjectives.Clear();

			(HasPickedUpMessage ? CompletedObjectives : Objectives).Add(string.Format("Pickup package from {0}." ,SourcePlanetDisplayName));
			(HasDeliveredMessage ? CompletedObjectives : Objectives).Add(string.Format("Deliver package to {0}.", DestinationPlanetDisplayName));
		}

		void CheckCloseToSource()
		{
			if (false == IsActive)
				return;

			if (true == HasPickedUpMessage)
				return;

			// Don't deploy another one if one already exists.
			if (null != MessageBoeyGameObject)
				return;

			if (string.IsNullOrEmpty(SourcePlanetUniqueName))
				return;

			StaticAgent sourcePlanet = StaticAgent.GetFromUniqueName(SourcePlanetUniqueName);
			if (null == sourcePlanet)
				return;

			float closestSqrDistance;
			DynamicAgent closestShip;
			Faction.PlayerFaction.ClosestShip(sourcePlanet.transform, out closestShip, out closestSqrDistance);

			if (null == closestShip)
				return;

			if (closestSqrDistance > PickupLaunchSqrDistance)
				return;

			GameObject messageBoey = GameObject.Instantiate(m_MessageBoeyPrefab, sourcePlanet.transform.position, Quaternion.identity) as GameObject;
			TargetedFollowingProjectile projectile = messageBoey.GetComponent<TargetedFollowingProjectile>();
			projectile.Target = closestShip.GetComponent<SpaceObject>();
			projectile.Source = sourcePlanet.GetComponent<SpaceObject>();
			projectile.SourceFaction = sourcePlanet.GetComponent<Membership>().Faction;
			projectile.OnDetonate += delegate {
				// This will be called when the missile "detonates" which in this case just sets this variable to true.
				HasPickedUpMessage = true;
			};

			MessageBoeyGameObject = messageBoey;

		}

		void CheckCloseToDestination()
		{
			if (false == IsActive)
				return;

			if (false == HasPickedUpMessage)
				return;

			if (true == HasDeliveredMessage)
				return;

			// Don't deploy another one if one already exists.
			if (null != MessageBoeyGameObject)
				return;

			StaticAgent destinationPlanet = StaticAgent.GetFromUniqueName(DestinationPlanetUniqueName);
			if (null == destinationPlanet)
				return;

			float closestSqrDistance;
			DynamicAgent closestShip;
			Faction.PlayerFaction.ClosestShip(destinationPlanet.transform, out closestShip, out closestSqrDistance);

			if (null == closestShip)
				return;

			if (closestSqrDistance > DeliverLaunchSqrDistance)
				return;

			//Debug.Log("Launch Message Boey");

			GameObject messageBoey = GameObject.Instantiate(m_MessageBoeyPrefab, closestShip.transform.position, Quaternion.identity) as GameObject;
			TargetedFollowingProjectile projectile = messageBoey.GetComponent<TargetedFollowingProjectile>();
			projectile.Target = destinationPlanet.GetComponent<SpaceObject>();
			projectile.Source = closestShip.GetComponent<SpaceObject>();
			projectile.SourceFaction = closestShip.GetComponent<Membership>().Faction;
			projectile.OnDetonate += delegate {
				// This will be called when the missile "detonates" which in this case just sets this variable to true.
				HasDeliveredMessage = true;
			};

			MessageBoeyGameObject = messageBoey;

		}

		void CheckComplete()
		{
			if (false == IsActive)
				return;

			if (false == MeetsCompleteConditions)
				return;

			if (true == IsComplete)
				return;
			
			// Add reward
			SaveStateManager.First.PlayerCredits += MonetaryReward;

			// Mark quest as complete.
			IsComplete = true;
		}

		#endregion Events
		#region Anccessors

		public override bool MeetsCompleteConditions
		{
			get
			{
				return true == HasPickedUpMessage && true == HasDeliveredMessage;
			}
		}

		#endregion Anccessors
	}
}