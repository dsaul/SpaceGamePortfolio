using UnityEngine;
using System.Collections;
using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;

namespace SpaceInsurgency.Items
{
	[AdvancedInspector]
	public class Base : SharedCode.Behaviours.InstanceTracked<Base>
	{
		#region Const

		public const string kCargoIronOre = "kCargoIronOre";

		public const string kWeaponLightBlaster = "kWeaponLightBlaster";
		public const string kWeaponMediumBlaster = "kWeaponMediumBlaster";
		public const string kWeaponHeavyBlaster = "kWeaponHeavyBlaster";
		public const string kWeaponVulcan = "kWeaponVulcan";
		public const string kWeaponRailgun = "kWeaponRailgun";
		public const string kWeaponLaserBolt = "kWeaponLaserBolt";
		public const string kWeaponSoloGun = "kWeaponSoloGun";
		public const string kWeaponIonCannon = "kWeaponIonCannon";
		public const string kWeaponHailChainGun = "kWeaponHailChainGun";
		public const string kWeaponParticleCannon = "kWeaponParticleCannon";
		public const string kWeaponQuadLightBlaster = "kWeaponQuadLightBlaster";
		public const string kWeaponSniperBeam = "kWeaponSniperBeam";

		public const string kShipModificationEngineSpeedIncreaseSmall = "kShipModificationEngineSpeedIncreaseSmall";
		public const string kShipModificationShieldRechargeRateSmall = "kShipModificationShieldRechargeRateSmall";
		public const string kShipModificationShieldCapacityIncreaseSmall = "kShipModificationShieldCapacityIncreaseSmall";
		public const string kShipModificationEnergyRechargeRateSmall = "kShipModificationEnergyRechargeRateSmall";
		public const string kShipModificationEnergyCapacityIncreaseSmall = "kShipModificationEnergyCapacityIncreaseSmall";
		public const string kShipModificationHealthRechargeRateSmall = "kShipModificationHealthRechargeRateSmall";
		public const string kShipModificationHealthCapacityIncreaseSmall = "kShipModificationHealthCapacityIncreaseSmall";
		public const string kShipModificationSensorRangeIncreaseSmall = "kShipModificationSensorRangeIncreaseSmall";

		public const string kClassSoldierWeaponsAccess = "kClassSoldierWeaponsAccess";
		public const string kSoldierDamageIncrease = "kSoldierDamageIncrease";
		public const string kPilotEngineSpeedIncrease = "kPilotEngineSpeedIncrease";
		public const string kPilotFreeShipQuest = "kPilotFreeShipQuest";
		public const string kMerchantPriceDecrease = "kMerchantPriceDecrease";
		public const string kMerchantIncomePerSecond = "kMerchantIncomePerSecond";
		public const string kGovernorFreeSpaceStation = "kGovernorFreeSpaceStation";
		public const string kGovernorEasierDialogue = "kGovernorEasierDialogue";
		public const string kEngineerEnergyCostDecrease = "kEngineerEnergyCostDecrease";
		public const string kEngineerRepairinFTL = "kEngineerRepairinFTL";

		public const string kPlayerAvatarLocation = "kPlayerAvatarLocation";
		#endregion Const
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
		float m_Weight = 0f;
		[Inspect, Group("Information")]
		public float Weight
		{
			get { return m_Weight; }
			set { m_Weight = value; }
		}

		[SerializeField]
		Sprite m_InventorySprite;
		[Inspect, Group("Information")]
		public Sprite InventorySprite
		{
			get { return m_InventorySprite; }
			set { m_InventorySprite = value; }
		}


		[SerializeField]
		Sprite m_ActionSprite;
		[Inspect, Group("Information")]
		public Sprite ActionSprite
		{
			get { return m_ActionSprite; }
			set { m_ActionSprite = value; }
		}

		[SerializeField]
		float m_AreaOfEffectRange;
		[Inspect, Group("Weapon")]
		public float AreaOfEffectRange
		{
			get { return m_AreaOfEffectRange; }
			set { m_AreaOfEffectRange = value; }
		}
		[SerializeField]
		bool m_CanActivate = false;
		[Inspect, Group("Activation")]
		public bool CanActivate
		{
			get { return m_CanActivate; }
			set { m_CanActivate = value; }
		}

		[SerializeField]
		bool m_CanBeSold = false;
		[Inspect, Group("Value")]
		public bool CanBeSold
		{
			get { return m_CanBeSold; }
			set { m_CanBeSold = value; }
		}

		[SerializeField]
		bool m_CanTarget = false;
		[Inspect, Group("Activation")]
		public bool CanTarget
		{
			get { return m_CanTarget; }
			set { m_CanTarget = value; }
		}

		[SerializeField]
		bool m_CanToggle = false;
		[Inspect, Group("Activation")]
		public bool CanToggle
		{
			get { return m_CanToggle; }
			set { m_CanToggle = value; }
		}

		[SerializeField]
		float m_Cost = 0;
		[Inspect, Group("Value")]
		public float Cost
		{
			get { return m_Cost; }
			set { m_Cost = value; }
		}

		[SerializeField]
		float m_CreditsPerSecondModifier = 0f;
		[Inspect, Group("Global Stats")]
		public float CreditsPerSecondModifier
		{
			get { return m_CreditsPerSecondModifier; }
			set { m_CreditsPerSecondModifier = value; }
		}

		[SerializeField]
		float m_Damage = 0f;
		[Inspect, Group("Weapon")]
		public float Damage
		{
			get { return m_Damage; }
			set { m_Damage = value; }
		}

		[SerializeField]
		string m_Description = "DESCRIPTION";
		[Inspect, Group("Information")]
		public string Description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}

		[SerializeField]
		float m_EnergyCost = 0;
		[Inspect, Group("Weapon")]
		public float EnergyCost
		{
			get { return m_EnergyCost; }
			set { m_EnergyCost = value; }
		}

		[SerializeField, Group("Dynamic Agent Stats")]
		float m_EnergyCostsMultiplier = 1f;
		[Inspect, Group("Dynamic Agent Stats")]
		public float EnergyCostsMultiplier
		{
			get { return m_EnergyCostsMultiplier; }
			set { m_EnergyCostsMultiplier = value; }
		}

		[SerializeField]
		float m_EnergyRechargeModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float EnergyRechargeModifier
		{
			get { return m_EnergyRechargeModifier; }
			set { m_EnergyRechargeModifier = value; }
		}

		[SerializeField]
		float m_FireMaxRange;
		[Inspect, Group("Weapon")]
		public float FireMaxRange
		{
			get { return m_FireMaxRange; }
			set { m_FireMaxRange = value; }
		}

		[SerializeField]
		float m_FireMinRange;
		[Inspect, Group("Weapon")]
		public float FireMinRange
		{
			get { return m_FireMinRange; }
			set { m_FireMinRange = value; }
		}

		[SerializeField]
		string m_FriendlyName;
		[Inspect, Group("Information")]
		public string FriendlyName
		{
			get { return m_FriendlyName; }
			set { m_FriendlyName = value; }
		}
		public string DisplayName
		{
			get { return m_FriendlyName; }
		}

		[SerializeField]
		string m_TooltipText;
		[Inspect, Group("Information")]
		public virtual string TooltipText
		{
			get
			{
				if (false == string.IsNullOrEmpty(m_TooltipText) || Application.isPlaying == false)
					return m_TooltipText;

				m_TooltipText = string.Format("<b>{0}</b>\n<i>{1}</i>\n<color=#ffd700ff>\u20A1 {2:0}</color>\n{3} lbs/item", FriendlyName, Description, Cost, Weight);
				return m_TooltipText;
				
			}
			set { m_TooltipText = value; }
		}

		[SerializeField]
		float m_HealthRechargeModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float HealthRechargeModifier
		{
			get { return m_HealthRechargeModifier; }
			set { m_HealthRechargeModifier = value; }
		}

		[SerializeField]
		bool m_IsClassItem;
		[Inspect, Group("Information")]
		public bool IsClassItem
		{
			get { return m_IsClassItem; }
			set { m_IsClassItem = value; }
		}

		[SerializeField]
		float m_MaxChannelTime;
		[Inspect, Group("Activation")]
		public float MaxChannelTime
		{
			get { return m_MaxChannelTime; }
			set { m_MaxChannelTime = value; }
		}

		[SerializeField]
		float m_MaxCooldownTime;
		[Inspect, Group("Activation")]
		public float MaxCooldownTime
		{
			get { return m_MaxCooldownTime; }
			set { m_MaxCooldownTime = value; }
		}

		[SerializeField]
		float m_MaxEnergyModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float MaxEnergyModifier
		{
			get { return m_MaxEnergyModifier; }
			set { m_MaxEnergyModifier = value; }
		}

		[SerializeField]
		float m_MaxHealthModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float MaxHealthModifier
		{
			get { return m_MaxHealthModifier; }
			set { m_MaxHealthModifier = value; }
		}

		[SerializeField]
		float m_MaxSensorRangeModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float MaxSensorRangeModifier
		{
			get { return m_MaxSensorRangeModifier; }
			set { m_MaxSensorRangeModifier = value; }
		}

		[SerializeField]
		float m_MaxShieldModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float MaxShieldModifier
		{
			get { return m_MaxShieldModifier; }
			set { m_MaxShieldModifier = value; }
		}

		[SerializeField]
		float m_MaxSpeedModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float MaxSpeedModifier
		{
			get { return m_MaxSpeedModifier; }
			set { m_MaxSpeedModifier = value; }
		}

		[SerializeField]
		float m_OrbitMaxRange;
		[Inspect, Group("Weapon")]
		public float OrbitMaxRange
		{
			get { return m_OrbitMaxRange; }
			set { m_OrbitMaxRange = value; }
		}

		[SerializeField]
		float m_OrbitMinRange;
		[Inspect, Group("Weapon")]
		public float OrbitMinRange
		{
			get { return m_OrbitMinRange; }
			set { m_OrbitMinRange = value; }
		}

		[SerializeField]
		float m_ProjectileLifetime;
		[Inspect, Group("Weapon")]
		public float ProjectileLifetime
		{
			get { return m_ProjectileLifetime; }
			set { m_ProjectileLifetime = value; }
		}

		[SerializeField]
		float m_ProjectileSpeed;
		[Inspect, Group("Weapon")]
		public float ProjectileSpeed
		{
			get { return m_ProjectileSpeed; }
			set { m_ProjectileSpeed = value; }
		}

		[SerializeField]
		bool m_RequiresLineOfSight;
		[Inspect, Group("Weapon")]
		public bool RequiresLineOfSight
		{
			get { return m_RequiresLineOfSight; }
			set { m_RequiresLineOfSight = value; }
		}

		[SerializeField]
		float m_ShieldRechargeModifier;
		[Inspect, Group("Dynamic Agent Stats")]
		public float ShieldRechargeModifier
		{
			get { return m_ShieldRechargeModifier; }
			set { m_ShieldRechargeModifier = value; }
		}

		[SerializeField]
		float m_StorePriceMultiplier = 1f;
		[Inspect, Group("Global Stats")]
		public float StorePriceMultiplier
		{
			get { return m_StorePriceMultiplier; }
			set { m_StorePriceMultiplier = value; }
		}

		[SerializeField]
		float m_TargetDetonateRange;
		[Inspect, Group("Weapon")]
		public float TargetDetonateRange
		{
			get { return m_TargetDetonateRange; }
			set { m_TargetDetonateRange = value; }
		}

		[SerializeField]
		bool m_DoTargetSelectFirst = false;
		[Inspect, Group("Activation")]
		public bool DoTargetSelectFirst
		{
			get { return m_DoTargetSelectFirst; }
			set { m_DoTargetSelectFirst = value; }
		}

		[SerializeField]
		Weapons.AttackType m_AttackType = Weapons.AttackType.None;
		[Inspect, Group("Weapon")]
		public Weapons.AttackType AttackType
		{
			get { return m_AttackType; }
			set { m_AttackType = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToSensors = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToSensors
		{
			get { return m_CanBeAddedToSensors; }
			set { m_CanBeAddedToSensors = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToEngines = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToEngines
		{
			get { return m_CanBeAddedToEngines; }
			set { m_CanBeAddedToEngines = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToEnergy = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToEnergy
		{
			get { return m_CanBeAddedToEnergy; }
			set { m_CanBeAddedToEnergy = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToHealth = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToHealth
		{
			get { return m_CanBeAddedToHealth; }
			set { m_CanBeAddedToHealth = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToShield = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToShield
		{
			get { return m_CanBeAddedToShield; }
			set { m_CanBeAddedToShield = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToWeapons = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToWeapons
		{
			get { return m_CanBeAddedToWeapons; }
			set { m_CanBeAddedToWeapons = value; }
		}

		[SerializeField]
		bool m_CanBeAddedToCargo = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToCargo
		{
			get { return m_CanBeAddedToCargo; }
			set { m_CanBeAddedToCargo = value; }
		}

#warning implement
		[SerializeField]
		bool m_CanBeAddedToGlobal = false;
		[Inspect, Group("Can Be Added To Components")]
		public bool CanBeAddedToGlobal
		{
			get { return m_CanBeAddedToGlobal; }
			set { m_CanBeAddedToGlobal = value; }
		}

		#endregion Variables
		#region Anccessors

		[Inspect, Group("Activation")]
		public virtual bool IsConditionallyEnabled()
		{
			return true;
		}



		public virtual bool IsActivationAppropriate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
			return false;
		}

		public virtual void OnActionClick()
		{

		}

		public void ActivateIfAppropriate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{
			// if target is blown up, then stop firing at it.
			if (true == target.GetComponent<Health>().IsDestroyed)
				return;

			// can't fire if blown up
			if (true == source.GetComponent<Health>().IsDestroyed)
				return;

			// can't fire if item is toggled off
			if (false == entry.ToggledOn)
				return;

			// must be within ranges
			if (targetSqrDistance > (FireMaxRange * FireMaxRange))
				return;
			if (targetSqrDistance < (FireMinRange * FireMinRange))
				return;

			// can't fire if we haven't cooled down yet
			if (Time.time < entry.LastCooldown + MaxCooldownTime)
				return;

#warning planets don't seem to stop this, ships do tho
			if (true == RequiresLineOfSight)
			{
				// If there is something in between the source and the target, we should not fire.
				bool blocked = false;
				RaycastHit hit;
				if (true == Physics.Linecast(source.transform.position, target.transform.position, out hit))
				{
					if (null != hit.collider)
					{
						SpaceObject hitSpaceObject = hit.collider.GetComponent<SpaceObject>();
						if (null != hitSpaceObject)
						{
							SpaceObject rootHitSpaceObject = hitSpaceObject.Root;
							if (rootHitSpaceObject != target)
							{
								//Debug.Log("particle hit " + rootHitSpaceObject, rootHitSpaceObject);
								blocked = true;
							}
						}
					}
				}

				if (true == blocked)
					return;
			}

			// Check to see if this weapon should activate in this situation.
			if (false == IsActivationAppropriate(source, entry, target, targetSqrDistance))
				return;

			// must have enough energy
			if (false == source.GetComponent<Energy>().LowerEnergy(EnergyCost))
				return;

			// Do the activation.
			Activate(source, entry, target, targetSqrDistance);

			// Go on cooldown.
			entry.LastCooldown = Time.time;
		}

		protected virtual void Activate(EquippableComponent source, EquippableComponent.Entry entry, SpaceObject target, float targetSqrDistance)
		{

		}

		public virtual void OnRemovedFrom(EquippableComponent newContainer)
		{

		}

		public virtual void OnAddedTo(EquippableComponent newContainer)
		{

		}

		#endregion Anccessors
	}
}
