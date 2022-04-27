using UnityEngine;
using System;
using System.Collections;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class FTL : SharedCode.Behaviours.Base
	{
		#region Variables

		float m_AnimationSpeed = 50f;

		[Inspect, ReadOnly, Group("Editor")]
		public float AnimationSpeed
		{
			get { return m_AnimationSpeed; }
			set { m_AnimationSpeed = value; }
		}

		
		bool m_FTLActive = false;

		[Inspect, ReadOnly, Group("Runtime")]
		public bool FTLActive
		{
			get { return m_FTLActive; }
			set { m_FTLActive = value; }
		}

		Vector3 m_FTLInStartPoint;

		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 FTLInStartPoint
		{
			get { return m_FTLInStartPoint; }
			set { m_FTLInStartPoint = value; }
		}
		
		Vector3 m_FTLInStartPointWorld;

		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 FTLInStartPointWorld
		{
			get { return m_FTLInStartPointWorld; }
			set { m_FTLInStartPointWorld = value; }
		}

		Vector3 m_FTLInDestpoint;

		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 FTLInDestpoint
		{
			get { return m_FTLInDestpoint; }
			set { m_FTLInDestpoint = value; }
		}

		float? m_FTLInParticleSpawnTime;

		[Inspect, ReadOnly, Group("Runtime")]
		public float? FTLInParticleSpawnTime
		{
			get { return m_FTLInParticleSpawnTime; }
			set { m_FTLInParticleSpawnTime = value; }
		}

		Vector3 m_FTLOutStartPoint;

		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 FTLOutStartPoint
		{
			get { return m_FTLOutStartPoint; }
			set { m_FTLOutStartPoint = value; }
		}

		Vector3 m_FTLOutDestPoint;

		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 FTLOutDestPoint
		{
			get { return m_FTLOutDestPoint; }
			set { m_FTLOutDestPoint = value; }
		}

		Vector3 m_FTLOutDestPointWorld;

		[Inspect, ReadOnly, Group("Runtime")]
		public Vector3 FTLOutDestPointWorld
		{
			get { return m_FTLOutDestPointWorld; }
			set { m_FTLOutDestPointWorld = value; }
		}

		#endregion Variables
		#region FTL IN

		[Inspect, Group("Utility")]
		public void DoFTLIn()
		{
			m_FTLActive = true;
			GetComponent<Engines>().enabled = false;
			GetComponent<DynamicAgent>().Parts.DisableColliders();
			GetComponent<DynamicAgent>().Parts.DisableVisuals();

			m_FTLInStartPoint = new Vector3(0, 0, -20);
			m_FTLInStartPointWorld = transform.TransformPoint(m_FTLInStartPoint);
			m_FTLInDestpoint = Vector3.zero;


			GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition = m_FTLInStartPoint;

			GameObject.Instantiate(FTLManager.Main.EffectFTLInPrefab, m_FTLInStartPointWorld, Quaternion.identity);
			AudioManager.Main.PlayClipAtPosition(FTLManager.Main.AudioFTLEntrance, AudioManager.Main.mixerFTL, m_FTLInStartPointWorld);
			m_FTLInParticleSpawnTime = Time.time;

			SetTimer(0.25f, true, FTLInWaitForSpawnEffect);
		}

		void FTLInWaitForSpawnEffect()
		{
			// If we change scenes mid ftl in we don't want any errors.
			if (null == GetComponent<DynamicAgent>())
			{
				ClearTimer(FTLInWaitForSpawnEffect);
				return;
			}
			
			if (Time.time < (m_FTLInParticleSpawnTime.Value + 0.75f))
				return;

			GetComponent<DynamicAgent>().Parts.EnableVisuals();

			ClearTimer(FTLInWaitForSpawnEffect);
			TimeManager.Main.OnTimeTickUpdate += FTLInAnimateShip;
		}

		void FTLInAnimateShip()
		{
			float step = m_AnimationSpeed * Time.deltaTime;
			GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition = Vector3.MoveTowards(GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition, m_FTLInDestpoint, step);

			if (GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition == m_FTLInDestpoint)
			{
				TimeManager.Main.OnTimeTickUpdate -= FTLInAnimateShip;
				GetComponent<DynamicAgent>().Parts.EnableColliders();
				GetComponent<Engines>().enabled = true;
				m_FTLActive = false;
			}
		}

		#endregion FTL IN
		#region FTL OUT

		[Inspect, Group("Utility")]
		public void DoFTLOut()
		{
			m_FTLActive = true;
			GetComponent<Engines>().enabled = false;
			GetComponent<DynamicAgent>().Parts.DisableColliders();

			m_FTLOutStartPoint = Vector3.zero;
			m_FTLOutDestPoint = new Vector3(0, 0, 20);
			m_FTLOutDestPointWorld = transform.TransformPoint(m_FTLOutDestPoint);

			GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition = m_FTLOutStartPoint;

			TimeManager.Main.OnTimeTickUpdate += FTLOutAnimateShip;
		}

		void FTLOutAnimateShip()
		{
			float step = m_AnimationSpeed * Time.deltaTime;
			GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition = Vector3.MoveTowards(GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition, m_FTLOutDestPoint, step);

			if (GetComponent<DynamicAgent>().AnimationContainer.transform.localPosition == m_FTLOutDestPoint)
			{
				TimeManager.Main.OnTimeTickUpdate -= FTLOutAnimateShip;

				GameObject.Instantiate(FTLManager.Main.EffectFTLOutPrefab, m_FTLOutDestPointWorld, Quaternion.identity);
				AudioManager.Main.PlayClipAtPosition(FTLManager.Main.AudioFTLExit, AudioManager.Main.mixerFTL, m_FTLOutDestPointWorld);
				if (null != GetComponent<DynamicAgent>())
					GetComponent<DynamicAgent>().CleanRemove();
				m_FTLActive = false;
			}
		}

		#endregion FTL OUT
		#region Serialization

		[Serializable]
		public class Serialized
		{
			
			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{

		}

		#endregion Serialization
	}



































}
