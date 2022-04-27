using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class TriggerDetectWithin : MonoBehaviour
	{
		#region Event Definitions

		public event Action<SpaceObject> OnObjectAdded; // SpaceObject obj
		public event Action<SpaceObject> OnObjectRemoved; // SpaceObject obj

		#endregion Event Definitions
		#region Variables

		[SerializeField]
		bool m_Debug = false;
		[Inspect]
		public bool DebugBefore
		{
			get { return m_Debug; }
			set { m_Debug = value; }
		}

		HashSet<SpaceObject> m_ObjectsInArea = new HashSet<SpaceObject>();
		[Inspect]
		public HashSet<SpaceObject> ObjectsInArea
		{
			get { return m_ObjectsInArea; }
			set { m_ObjectsInArea = value; }
		}

		#endregion Variables
		#region Setup

		void Awake()
		{
			OnObjectAdded = new Action<SpaceObject>(DefaultOnObjectAdded);
			OnObjectRemoved = new Action<SpaceObject>(DefaultOnObjectRemoved);
			GetComponent<Collider>().isTrigger = true;
		}

		#endregion Setup
		#region Events

		void OnTriggerEnter(Collider other)
		{
			OnTriggerStay(other);
		}

		void OnTriggerStay(Collider other)
		{
			Assert.IsNotNull<HashSet<SpaceObject>>(m_ObjectsInArea);

			if (null == other)
				return;
			if (other.tag == Const.Tags.Ignore)
				return;

			SpaceObject spaceObject = other.GetComponent<SpaceObject>();
			if (null == spaceObject)
				return;

			spaceObject = spaceObject.Root;

			if (false == m_ObjectsInArea.Contains(spaceObject))
				OnObjectAdded(spaceObject);
		}

		void DefaultOnObjectAdded(SpaceObject obj)
		{
			if (true == m_Debug)
				Debug.Log("OnObjectAdded " + obj, obj);
			m_ObjectsInArea.Add(obj);
		}

		void OnTriggerExit(Collider other)
		{
			Assert.IsNotNull<HashSet<SpaceObject>>(m_ObjectsInArea);

			if (null == other)
				return;
			if (other.tag == Const.Tags.Ignore)
				return;

			SpaceObject spaceObject = other.GetComponent<SpaceObject>();
			if (null == spaceObject)
				return;

			spaceObject = spaceObject.Root;

			if (true == m_ObjectsInArea.Contains(spaceObject))
				OnObjectRemoved(spaceObject);
		}

		void DefaultOnObjectRemoved(SpaceObject obj)
		{
			if (true == m_Debug)
				Debug.Log("OnObjectRemoved " + obj, obj);
			m_ObjectsInArea.Remove(obj);
		}

		#endregion Events
	}
}

