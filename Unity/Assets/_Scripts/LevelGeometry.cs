using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class LevelGeometry : SharedCode.Behaviours.InstanceTracked<LevelGeometry>
	{
		#region Static Delegates

		static Action<LevelGeometry, bool> _OnLevelGeometryCreated = null;
		public static Action<LevelGeometry, bool> OnLevelGeometryCreated // LevelGeometry source, bool fromSaveFile
		{
			get
			{
				if (null == _OnLevelGeometryCreated)
					_OnLevelGeometryCreated = new Action<LevelGeometry, bool>(delegate { });
				return _OnLevelGeometryCreated;
			}
			set
			{
				_OnLevelGeometryCreated = value;
			}
		}

		#endregion Static Delegates
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
		float m_MinDistance = 5f;

		[Inspect, Group("Editor")]
		public float MinDistance
		{
			get { return m_MinDistance; }
			set { m_MinDistance = value; }
		}

		[SerializeField]
		float m_MaxDistance = 60f;

		[Inspect, Group("Editor")]
		public float MaxDistance
		{
			get { return m_MaxDistance; }
			set { m_MaxDistance = value; }
		}

		[SerializeField]
		float m_MinTilt = 0f;

		[Inspect, Group("Editor")]
		public float MinTilt
		{
			get { return m_MinTilt; }
			set { m_MinTilt = value; }
		}

		[SerializeField]
		float m_MaxTilt = 90f;

		[Inspect, Group("Editor")]
		public float MaxTilt
		{
			get { return m_MaxTilt; }
			set { m_MaxTilt = value; }
		}

		[SerializeField]
		bool m_IsGalaxyMap = false;

		[Inspect, Group("Editor")]
		public bool IsGalaxyMap
		{
			get { return m_IsGalaxyMap; }
			set { m_IsGalaxyMap = value; }
		}

		#endregion Variables
		#region Level Loading

		static bool loadIsFromSaveFile = false;

		public static IEnumerator LoadAsync(PlanetarySystem system, bool fromSaveFile = false)
		{
			loadIsFromSaveFile = fromSaveFile;

			Assert.IsNotNull<PlanetarySystem>(system);
			Assert.IsFalse(string.IsNullOrEmpty(system.DisplayName));

			yield return Application.LoadLevelAsync(system.SceneName);

			// If we didn't load any level data something went wrong.
			Assert.AreNotEqual<int>(0, LevelGeometry.InstanceCount);

			// Enable the level objects.
			List<LevelGeometry> instances = LevelGeometry.Instances;
			for (int i = 0; i < instances.Count; i++)
				instances[i].gameObject.SetActive(true);

			yield break;
		}
		
		public static void RemoveAll()
		{
			// Before we delete everything we need to deselect things so that we don't get null reference errors.
			SelectionManager.Main.DeSelectAll();
			
			// Remove all active ships.
			while (DynamicAgent.InstanceCount > 0)
				GameObject.DestroyImmediate(DynamicAgent.First.gameObject);

			// Remove all active missiles.
			while (TargetedFollowingProjectile.InstanceCount > 0)
				GameObject.DestroyImmediate(TargetedFollowingProjectile.First.gameObject);

			while (TargetedNonFollowingProjectile.InstanceCount > 0)
				GameObject.DestroyImmediate(TargetedNonFollowingProjectile.First.gameObject);

			// Destroy all level geometry.
			while (LevelGeometry.InstanceCount > 0)
				GameObject.DestroyImmediate(LevelGeometry.First.gameObject);
		}

		#endregion Level Loading
		#region Setup
		protected override void Start()
		{
			base.Start();

			// Abort loading if no gamemanager. Try to load the first level.
			if (null == SaveStateManager.Main)
			{
				Application.LoadLevel(0);
				return;
			}

			OnLevelGeometryCreated(this, loadIsFromSaveFile);
		}

		#endregion Setup
		#region Utility

		public static void RaycastObjectsAndMovementLocation(ref List<SpaceObject> List, out Vector3 MovementLocation, Camera Camera = null)
		{
			if (null == Camera)
				Camera = CameraManager.Main.CameraGame;
			if (null != List)
				List.Clear();
			MovementLocation = Const.Vector3.Sentinel;

			Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

			RaycastHit[] hits = Physics.RaycastAll(ray);


			for (int i = 0; i < hits.Length; i++)
			{

				RaycastHit hit = hits[i];
				Collider collider = hit.collider;
				SpaceObject spaceObject = collider.GetComponent<SpaceObject>();
				if (null != spaceObject)
				{
					SpaceObject rootSpaceObject = spaceObject.Root;
					if (null != rootSpaceObject)
						if (null != List)
							if (false == List.Contains(rootSpaceObject))
								List.Add(rootSpaceObject);
				}

				MovementPlane movementPlane = collider.GetComponent<MovementPlane>();
				if (null != movementPlane)
				{
					MovementLocation = hit.point;
				}
			}


			List.Sort(delegate(SpaceObject x, SpaceObject y) { return x.transform.position.SqrDistance(Camera.transform.position).CompareTo(y.transform.position.SqrDistance(Camera.transform.position)); });

		}

		public static void SpherecastObjectsAndMovementLocation(ref List<SpaceObject> List, out Vector3 MovementLocation, Camera Camera = null, float Radius = 0.5f)
		{
			if (null == Camera)
				Camera = CameraManager.Main.CameraGame;
			if (null != List)
				List.Clear();
			MovementLocation = Const.Vector3.Sentinel;

			Ray ray = CameraManager.Main.CameraGame.ScreenPointToRay(Input.mousePosition);

			RaycastHit[] hits = Physics.SphereCastAll(ray, 0.5f);

			for (int i = 0; i < hits.Length; i++)
			{

				RaycastHit hit = hits[i];
				Collider collider = hit.collider;
				SpaceObject spaceObject = collider.GetComponent<SpaceObject>();
				if (null != spaceObject)
				{
					SpaceObject rootSpaceObject = spaceObject.Root;
					if (null != rootSpaceObject)
						if (null != List)
							if (false == List.Contains(rootSpaceObject))
								List.Add(rootSpaceObject);
				}

				MovementPlane movementPlane = collider.GetComponent<MovementPlane>();
				if (null != movementPlane)
				{
					MovementLocation = hit.point;
				}
			}


			List.Sort(delegate(SpaceObject x, SpaceObject y) { return x.transform.position.SqrDistance(Camera.transform.position).CompareTo(y.transform.position.SqrDistance(Camera.transform.position)); });

		}

		#endregion Utility
































	}
}