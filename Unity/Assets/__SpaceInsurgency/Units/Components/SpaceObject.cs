using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using SpaceInsurgency;
using SharedCode;
using AdvancedInspector;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class SpaceObject : SharedCode.Behaviours.InstanceTracked<SpaceObject>
	{
		#region Types / Const

		public enum DamageType
		{
			Regular = 0,
			EMP = 1
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

		[Inspect, Group("Editor")]
		public bool ForceRoot = false;

		[Inspect, Group("Runtime")]
		public bool IsSelectable
		{
			get
			{
				return null != GetComponent<DynamicAgent>() || null != GetComponent<StaticAgent>() || null != GetComponent<PickupAgent>();
			}
		}

		bool m_IsMouseOverFudged = false;

		[Inspect, Group("Runtime")]
		public bool IsMouseOverFudged
		{
			get { return m_IsMouseOverFudged; }
		}

		bool m_IsSelected = false;
		[Inspect, Group("Runtime")]
		public bool IsSelected
		{
			get { return m_IsSelected; }
		}

		bool m_DebugMessages = false;
		[Inspect, Group("Runtime")]
		public bool DebugMessages
		{
			get { return m_DebugMessages; }
			set { m_DebugMessages = value; }
		}


		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();

			if (null != GetComponent<AudioSource>())
			{
				GetComponent<AudioSource>().minDistance = 10;
			}

			// We don't do this now as it is causing lag when spawning things.
			// Check sub objects to see if they have space objects, if not create them and then set the parent to this.
			/*for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				SpaceObject spaceObject = child.GetComponent<SpaceObject>();
				if (null == spaceObject)
				{
					spaceObject = child.gameObject.AddComponent<SpaceObject>();
				}
			}*/
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
			if (true == signalSub)
				return;
			if (null == InputManager.Main)
				return;
			if (null == SelectionManager.Main)
				return;

			InputManager.Main.OnMouseoverSpaceObjectFudgedChanged += InputManager_OnMouseoverSpaceObjectFudgedChanged;
			SelectionManager.Main.OnSelectionAdded += SelectionManager_OnSelectionAdded;
			SelectionManager.Main.OnSelectionRemoved += SelectionManager_OnSelectionRemoved;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == InputManager.Main)
				return;
			if (null == SelectionManager.Main)
				return;

			InputManager.Main.OnMouseoverSpaceObjectFudgedChanged -= InputManager_OnMouseoverSpaceObjectFudgedChanged;
			SelectionManager.Main.OnSelectionAdded -= SelectionManager_OnSelectionAdded;
			SelectionManager.Main.OnSelectionRemoved -= SelectionManager_OnSelectionRemoved;

			signalSub = false;
		}

		#endregion Setup
		#region Events

		void InputManager_OnMouseoverSpaceObjectFudgedChanged(InputManager source, List<SpaceObject> objects)
		{
			m_IsMouseOverFudged = objects.Contains(Root);
		}


		void SelectionManager_OnSelectionAdded(SelectionManager sender, SpaceObject obj)
		{
			if (obj == this)
				m_IsSelected = true;
		}

		void SelectionManager_OnSelectionRemoved(SelectionManager sender, SpaceObject obj)
		{
			if (obj == this)
				m_IsSelected = false;
		}

		#endregion Events
		#region Damage

		public float TakeDamage(float damage, DamageType damageType)
		{
			Assert.IsFalse(damage < 0);

			float shieldDamageTaken = 0f;
			float armourDamageTaken = 0f;
			float remainingDamage = damage;

			// Damage first hits the dampening shield.
			Shield shieldGenerator = GetComponent<Shield>();
			if (null != shieldGenerator)
			{
				shieldGenerator.TakeDamage(remainingDamage, damageType, out remainingDamage, out shieldDamageTaken);
			}

			// Then it hits the ship's armour.
			if (remainingDamage > 0)
			{
				Health armour = GetComponent<Health>();
				if (null != armour)
				{
					armour.TakeDamage(remainingDamage, damageType, out remainingDamage, out armourDamageTaken);
				}
			}

			return remainingDamage;
		}

		#endregion
		#region Main

		// Anything in here has to be super optimized. 
		protected override void Update()
		{
			base.Update();

			// If we aren't the root and parent is something then we bail early.
			if (false == ForceRoot && null != Parent)
				return;

			// If we aren't selectable then we don't need to know the selectable information.
			if (false == IsSelectable)
				return;

			

			if (false == m_IsSelected && false == m_IsMouseOverFudged)
				return;
			
#warning this needs to be optimised
			CalculateScreenBounds();
		}

		#endregion Main
		#region Selection

		[Inspect, ReadOnly]
		public float? selectionLastMaxX;
		[Inspect, ReadOnly]
		public float? selectionLastMaxY;
		[Inspect, ReadOnly]
		public float? selectionLastMinX;
		[Inspect, ReadOnly]
		public float? selectionLastMinY;

		public void CalculateScreenBounds()
		{
			Camera cam = CameraManager.Main.CameraGame;
			
			// If it is a ship we need a specific base to begin mesh scanning so as to not get the thrusters.
			GameObject scanBase = gameObject;
			DynamicAgent dynamicAgent = GetComponent<DynamicAgent>();
			StaticAgent staticAgent = GetComponent<StaticAgent>();
			PickupAgent pickupAgent = GetComponent<PickupAgent>();
			if (null != dynamicAgent && null != dynamicAgent.Parts)
			{
				if (null != dynamicAgent.Parts.baseObjectForSelectionCalculation)
					scanBase = dynamicAgent.Parts.baseObjectForSelectionCalculation;
				else
					Debug.LogWarning("No selections bounds mesh assigned, figuring out bounds the expensive way.", gameObject);
			}
			else if (null != staticAgent)
			{
				if (null != staticAgent.SelectionBounds)
					scanBase = staticAgent.SelectionBounds;
				else
					Debug.LogWarning("No selections bounds mesh assigned, figuring out bounds the expensive way.", gameObject);
			}
			else if (null != pickupAgent)
			{
				if (null != pickupAgent.SelectionBounds)
					scanBase = pickupAgent.SelectionBounds;
				else
					Debug.LogWarning("No selections bounds mesh assigned, figuring out bounds the expensive way.", gameObject);
			}



			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;

			// Go through all the meshes getting the bounds.
			MeshFilter[] meshFilters = scanBase.GetComponentsInChildren<MeshFilter>();
			for (int i = 0; i < meshFilters.Length; i++)
			{
				MeshFilter meshFilter = meshFilters[i];
				Mesh sharedMesh = meshFilter.sharedMesh;
				Vector3[] verts = sharedMesh.vertices;
				//Debug.Log(verts.Length);
				for (int j = 0; j < verts.Length; j++)
				{
					Vector3 vert = verts[j];
					Vector3 vertWorld = meshFilter.transform.TransformPoint(vert);
					Vector3 vertScreen = cam.WorldToScreenPoint(vertWorld);

					if (vertScreen.x < minX)
						minX = vertScreen.x;
					if (vertScreen.y < minY)
						minY = vertScreen.y;
					if (vertScreen.x > maxX)
						maxX = vertScreen.x;
					if (vertScreen.y > maxY)
						maxY = vertScreen.y;
				}
			}

			// We floor the results.
			minX = Mathf.Floor(minX);
			minY = Mathf.Floor(minY);
			maxX = Mathf.Floor(maxX);
			maxY = Mathf.Floor(maxY);

			// Add a small buffer.
			minX -= 5f;
			minY -= 5f;
			maxX += 5f;
			maxY += 5f;

			// Shift it by half to get it onto pixel boundaries.
			//minX += 0.5f;
			//minY += 0.5f;
			//maxX -= 0.5f;
			//maxY -= 0.5f;

			// Store these in case other components need them.
			selectionLastMaxX = maxX;
			selectionLastMaxY = maxY;
			selectionLastMinX = minX;
			selectionLastMinY = minY;
		}



		#endregion Selection
		#region Ancessors

		
		[Inspect, Group("Runtime")]
		public SpaceObject Parent
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				Transform parentTransform = transform.parent;
				if (null == parentTransform)
					return null;

				SpaceObject parentSpaceObject = parentTransform.GetComponent<SpaceObject>();
				if (null != parentSpaceObject)
					return parentSpaceObject;

				// Is parent
				return null;
			}
		}

		[Inspect, Group("Runtime")]
		public SpaceObject Root
		{
			get
			{
				if (false == Application.isPlaying)
					return null;
				
				SpaceObject spob = this;

				while (null != spob.Parent)
				{
					if (spob.ForceRoot)
						return spob;
					spob = spob.Parent;
				}
				return spob;
			}
		}

		#endregion Ancessors
		#region Utility

		public static IEnumerable<SpaceObject> EnumerateActiveSpaceObjectsRootOnly()
		{
			using (IEnumerator<SpaceObject> enumerator = Instances.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (null == enumerator.Current.Parent)
						yield return enumerator.Current;
				}
			}
		}

		public static SpaceObject ClosestObjectTo(IEnumerable<SpaceObject> collection, Transform t)
		{
			SpaceObject closestSpaceObjectcollection = null;
			float lastDistance = float.MaxValue;

			using (IEnumerator<SpaceObject> enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SpaceObject so = enumerator.Current;

					float dist = so.transform.position.SqrDistance(t.position);
					//float dist = Vector3.Distance(so.transform.position, t.position);
					if (dist < lastDistance)
					{
						closestSpaceObjectcollection = so;
						lastDistance = dist;
					}

				}
			}

			return closestSpaceObjectcollection;
		}

		[Inspect, Group("Runtime")]
		public bool IsVisibleToCameraGame
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				if (null == CameraManager.Main)
					return false;
				Camera cam = CameraManager.Main.CameraGame;
				
				Renderer[] renderers = GetComponentsInChildren<Renderer>();
				for (int i = 0; i < renderers.Length; i++)
					if (renderers[i].IsVisibleFrom(cam))
						return true;
				return false;
			}
		}

		[Inspect, Group("Runtime")]
		public float SqrDistanceToGameCamera
		{
			get
			{
				if (false == Application.isPlaying)
					return float.MaxValue;
				if (null == CameraManager.Main)
					return float.MaxValue;
				return transform.position.SqrDistance(CameraManager.Main.CameraGame.transform.position);
			}
		}

		[Inspect, Group("Runtime")]
		public bool IsMouseOverExact
		{
			get
			{
				if (false == Application.isPlaying)
					return false;
				if (null == InputManager.Main)
					return false;
				return InputManager.Main.MouseOverSpaceObjectsExact.Contains(Root);
			}
		}

		public virtual IEnumerable<Base> ItemsShownInActionArea
		{
			get
			{
				if (null != GetComponent<Sensors>())
					using (IEnumerator<Base> e = GetComponent<Sensors>().ItemsShownInActionArea.GetEnumerator())
						while (e.MoveNext())
							yield return e.Current;
				if (null != GetComponent<Engines>())
					using (IEnumerator<Base> e = GetComponent<Engines>().ItemsShownInActionArea.GetEnumerator())
						while (e.MoveNext())
							yield return e.Current;
				if (null != GetComponent<Energy>())
					using (IEnumerator<Base> e = GetComponent<Energy>().ItemsShownInActionArea.GetEnumerator())
						while (e.MoveNext())
							yield return e.Current;
				if (null != GetComponent<Health>())
					using (IEnumerator<Base> e = GetComponent<Health>().ItemsShownInActionArea.GetEnumerator())
						while (e.MoveNext())
							yield return e.Current;
				if (null != GetComponent<Shield>())
					using (IEnumerator<Base> e = GetComponent<Shield>().ItemsShownInActionArea.GetEnumerator())
						while (e.MoveNext())
							yield return e.Current;
				if (null != GetComponent<Weapons>())
					using (IEnumerator<Base> e = GetComponent<Weapons>().ItemsShownInActionArea.GetEnumerator())
						while (e.MoveNext())
							yield return e.Current;
				yield break;
			}
		}

		#endregion Utility
		#region Serialization

		[Serializable]
		public class Serialized
		{
			public string uniqueName = null;

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();
			serialized.uniqueName = UniqueName;
			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			UniqueName = serialized.uniqueName;
		}

		#endregion Serialization

		public override string ToString()
		{
			return string.Format("[SpaceObject: {0} '{1}']", transform.position, name);
		}
	}
}















































