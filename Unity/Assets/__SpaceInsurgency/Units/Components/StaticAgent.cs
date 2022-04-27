using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SpaceInsurgency;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	
	[RequireComponent(typeof(SpaceObject))]
	[RequireComponent(typeof(Membership))]
	[AdvancedInspector]
	public class StaticAgent : SharedCode.Behaviours.InstanceTracked<StaticAgent>
	{
		#region Variables

		[SerializeField]
		string m_DisplayName = "Planet Name";
		[Inspect]
		public string DisplayName
		{
			get { return m_DisplayName; }
			set { m_DisplayName = value; }
		}

		[SerializeField]
		bool m_IsTradeDestination = false;
		[Inspect]
		public bool IsTradeDestination
		{
			get { return m_IsTradeDestination; }
			set { m_IsTradeDestination = value; }
		}

		[SerializeField]
		bool m_DisallowSelection = false;
		[Inspect]
		public bool DisallowSelection
		{
			get { return m_DisallowSelection; }
			set { m_DisallowSelection = value; }
		}

		[SerializeField]
		bool m_AllowAsLocalMessageDeliveryDestination = false;
		[Inspect]
		public bool AllowAsLocalMessageDeliveryDestination
		{
			get { return m_AllowAsLocalMessageDeliveryDestination; }
			set { m_AllowAsLocalMessageDeliveryDestination = value; }
		}

		[SerializeField]
		float m_IdleAndSpawnDistanceMin = 10f;
		[Inspect]
		public float IdleAndSpawnDistanceMin
		{
			get { return m_IdleAndSpawnDistanceMin; }
			set { m_IdleAndSpawnDistanceMin = value; }
		}

		[SerializeField]
		float m_IdleAndSpawnDistanceMax = 25f;
		[Inspect]
		public float IdleAndSpawnDistanceMax
		{
			get { return m_IdleAndSpawnDistanceMax; }
			set { m_IdleAndSpawnDistanceMax = value; }
		}

		[SerializeField]
		GameObject m_SelectionBounds;
		[Inspect]
		public GameObject SelectionBounds
		{
			get { return m_SelectionBounds; }
			set { m_SelectionBounds = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();
		}

		#endregion Setup
		#region Main

		[Inspect]
		void Select()
		{
			SelectionManager.Main.DeSelectAll();
			SelectionManager.Main.Select(GetComponent<SpaceObject>());
		}

		[Inspect]
		void Follow()
		{
			CameraManager.Main.CameraGame.GetComponent<RtsCamera>().Follow(transform);
		}

		[Inspect]
		void SelectAndFollow()
		{
			Select();
			Follow();
		}

		#endregion Main
		#region Anccessors

		public Vector3 GetNewIdleAndSpawnPoint()
		{
			float spawnDistMin = IdleAndSpawnDistanceMin;
			float spawnDistMax = IdleAndSpawnDistanceMax;
			float chosenDist = UnityEngine.Random.Range(spawnDistMin, spawnDistMax);
			float chosenAngle = UnityEngine.Random.Range(0, 360);
			//Debug.Log("chosenDist " + chosenDist);



			Vector3 localPoint = Vector3.forward.RotateAroundPivot(Vector3.zero, Quaternion.Euler(new Vector3(0, chosenAngle, 0)));
			//Debug.Log("localPoint " + localPoint);
			localPoint *= chosenDist;
			//Debug.Log("localPoint multiplied " + localPoint);
			Vector3 worldPoint = transform.TransformPoint(localPoint);
			worldPoint.y = 0f; // if planets are rotated it fucks everything with the navmesh up.
			//Debug.Log("worldPoint " + worldPoint);
			

			return worldPoint;
		}

		public Vector3 GetNewDepartPoint(int multiplier = 5)
		{
			return transform.GetRandomPointAroundTransformMultiplied(IdleAndSpawnDistanceMin * multiplier, IdleAndSpawnDistanceMax * multiplier, false);
		}

		#endregion Anccessors
	}




















































}