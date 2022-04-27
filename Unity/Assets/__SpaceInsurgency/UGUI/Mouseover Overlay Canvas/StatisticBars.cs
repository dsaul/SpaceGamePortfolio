using AdvancedInspector;
using SharedCode;
using SpaceInsurgency;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SpaceInsurgency.Mouseover
{
	[AdvancedInspector]
	public class StatisticBars : SharedCode.Behaviours.Base
	{
		#region Variables

		SpaceObject m_Target;

		[Inspect, ReadOnly, Group("Runtime")]
		public SpaceObject Target
		{
			get { return m_Target; }
			set
			{
				if (null != m_Target)
					UnSubSignal();
				m_Target = value;
				if (null != m_Target)
				{
					SubSignal();
				}
			}
		}

		[Inspect, Group("Editor")]
		public GameObject m_HealthRoot;

		[Inspect, Group("Editor")]
		public Image m_HealthFill;

		[Inspect, Group("Editor")]
		public GameObject m_EnergyRoot;

		[Inspect, Group("Editor")]
		public Image m_EnergyFill;

		[Inspect, Group("Editor")]
		public GameObject m_ShieldRoot;

		[Inspect, Group("Editor")]
		public Image m_ShieldFill;

		Image[] imageComponents;

		#endregion Variables
		#region Setup

		protected override void Awake()
		{
			base.Awake();

			

			//SubSignal();
		}

		protected override void Start()
		{
			base.Start();

			imageComponents = GetComponentsInChildren<Image>();
			for (int i = 0; i < imageComponents.Length; i++)
				imageComponents[i].enabled = false;

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

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (null == m_Target)
				return;
			if (true == signalSub)
				return;

			bool showStatBars = false;

			Health health = m_Target.GetComponent<Health>();
			if (null == health)
			{
				m_HealthRoot.SetActive(false);
			}
			else
			{
				m_HealthRoot.SetActive(true);
				showStatBars = true;
				health.OnHealthChanged += Target_OnHealthChanged;
				Target_OnHealthChanged(health, health.HealthCurrent);
			}

			Shield shield = m_Target.GetComponent<Shield>();
			if (null == shield)
			{
				m_ShieldRoot.SetActive(false);
			}
			else
			{
				m_ShieldRoot.SetActive(true);
				showStatBars = true;
				shield.OnShieldChanged += Target_OnShieldChanged;
				Target_OnShieldChanged(shield, shield.ShieldCurrent);
			}

			Energy energy = m_Target.GetComponent<Energy>();
			if (null == energy)
			{
				m_EnergyRoot.SetActive(false);
			}
			else
			{
				m_EnergyRoot.SetActive(true);
				showStatBars = true;
				energy.OnEnergyChanged += Target_OnEnergyChanged;
				Target_OnEnergyChanged(energy, energy.EnergyCurrent);
			}

			GetComponent<CanvasGroup>().alpha = showStatBars ? 1 : 0;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == m_Target)
				return;

			Health health = m_Target.GetComponent<Health>();
			if (null != health)
				health.OnHealthChanged -= Target_OnHealthChanged;

			Shield shield = m_Target.GetComponent<Shield>();
			if (null != shield)
				shield.OnShieldChanged -= Target_OnShieldChanged;

			Energy energy = m_Target.GetComponent<Energy>();
			if (null != energy)
				energy.OnEnergyChanged -= Target_OnEnergyChanged;

			signalSub = false;
		}

		#endregion Setup
		#region Events

		void Target_OnHealthChanged(Health source, float newHealth)
		{
			m_HealthFill.fillAmount = source.HealthCurrent / source.HealthMax;
		}

		void Target_OnShieldChanged(Shield source, float newShield)
		{
			m_ShieldFill.fillAmount = source.ShieldCurrent / source.ShieldMax;
		}

		void Target_OnEnergyChanged(Energy source, float newEnergy)
		{
			m_EnergyFill.fillAmount = source.EnergyCurrent / source.EnergyMax;
		}

		#endregion Events
		#region Main

		protected override void Update()
		{
			base.Update();

			if (null == m_Target)
				return;
			if (null == m_Target.selectionLastMinX)
				return;
			if (null == m_Target.selectionLastMinY)
				return;
			if (null == m_Target.selectionLastMaxX)
				return;
			if (null == m_Target.selectionLastMaxY)
				return;

			float minX = m_Target.selectionLastMinX.Value;
			float minY = m_Target.selectionLastMinY.Value;
			float maxX = m_Target.selectionLastMaxX.Value;
			float maxY = m_Target.selectionLastMaxY.Value;

			float swX = minX;
			float swY = minY;

			float seX = maxX;
			float seY = minY;

			float neX = maxX;
			float neY = maxY;

			float nwX = minX;
			float nwY = maxY;

			RectTransform rectTransform = GetComponent<RectTransform>();
			Assert.IsNotNull<RectTransform>(rectTransform);

			rectTransform.position = new Vector3(swX,swY-2,0);
			float width = Mathf.Floor(seX - swX);
			rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

			bool targetIsVisible = Target.IsVisibleToCameraGame;
			for (int i = 0; i < imageComponents.Length; i++)
				imageComponents[i].enabled = targetIsVisible;
		}

		#endregion Main
	}
}
