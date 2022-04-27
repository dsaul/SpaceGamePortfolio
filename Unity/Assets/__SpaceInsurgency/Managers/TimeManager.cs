using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class TimeManager : SharedCode.Behaviours.InstanceTracked<TimeManager>
	{
		#region Events

		public event Action OnTimeTickUpdate;
		public event Action OnTimeTickFixedUpdate;

		#endregion Events
		#region Types

		[AdvancedInspector]
		[Serializable]
		public class GameTime
		{
			[Inspect, ReadOnly]
			public int year;
			[Inspect, ReadOnly]
			public int month;
			[Inspect, ReadOnly]
			public int day;
			[Inspect, ReadOnly]
			public int hour;
			[Inspect, ReadOnly]
			public int minute;
			[Inspect, ReadOnly]
			public int second;

			public GameTime() { }
			public GameTime(int year, int month, int day, int hour, int minute, int second) : this()
			{
				this.year = year;
				this.month = month;
				this.day = day;
				this.hour = hour;
				this.minute = minute;
				this.second = second;
			}

			public DateTime DateTime
			{
				get
				{
					return new DateTime(year, month, day, hour, minute, second);
				}
			}

			[Inspect]
			public string InGameFormattedStringValue
			{
				get
				{
					return DateTime.ToString("MMM d, yyyy");
				}
			}

			public void AddSeconds(int seconds)
			{
				DateTime dt = new DateTime(year, month, day, hour, minute, second);
				DateTime dtMod = dt.AddSeconds(seconds);
				year = dtMod.Year;
				month = dtMod.Month;
				day = dtMod.Day;
				hour = dtMod.Hour;
				minute = dtMod.Minute;
				second = dtMod.Second;
			}
		}
		#endregion Types
		#region Variables

		float m_OriginalTimescale;
		[Inspect, ReadOnly]
		public float OriginalTimescale
		{
			get { return m_OriginalTimescale; }
			private set { m_OriginalTimescale = value; }
		}

		bool m_GameTimePaused = false;
		[Inspect]
		public bool GameTimePaused
		{
			get
			{
				return m_GameTimePaused;
			}
			set
			{
				if (m_GameTimePaused == value)
					return;
				m_GameTimePaused = value;

				Time.timeScale = true == m_GameTimePaused ? m_OriginalTimescale : 0f;
			}
		}

		#endregion Variables
		#region Setup
		
		protected override void Awake()
		{
			base.Awake();

			OnTimeTickUpdate = new Action(delegate { });
			OnTimeTickFixedUpdate = new Action(delegate { });

			m_OriginalTimescale = Time.timeScale;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SetTimer(1f, true, OnTimeTickOneSecond);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(OnTimeTickOneSecond);
		}

		#endregion Setup
		#region Anccessors
		
		

		#endregion Anccessors
		#region Main

		void OnTimeTickOneSecond()
		{
			if (false == enabled)
				return;

			// We want to time to go roughly 1 day per half hour.
			SaveStateManager.First.GameTime.AddSeconds(48);
		}

		protected override void Update()
		{
			base.Update();

			OnTimeTickUpdate();
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			OnTimeTickFixedUpdate();
		}

		#endregion Main
		
	}
}