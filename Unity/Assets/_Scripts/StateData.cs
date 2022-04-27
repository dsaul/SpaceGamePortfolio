using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using System.Xml.Serialization;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[Serializable]
	[AdvancedInspector]
	public class StateData
	{
		[Inspect]
		public TimeManager.GameTime gameTime = new TimeManager.GameTime(2553, 7, 3, 16, 30, 5);

		[Inspect]
		public List<DynamicAgent.Serialized> dynamicAgents = new List<DynamicAgent.Serialized>();

		[Inspect]
		public SerializableDictionary<string, int> intStore = new SerializableDictionary<string, int>();
		[Inspect]
		public SerializableDictionary<string, string> stringStore = new SerializableDictionary<string, string>();
		[Inspect]
		public SerializableDictionary<string, float> floatStore = new SerializableDictionary<string, float>();
		[Inspect]
		public SerializableDictionary<string, bool> boolStore = new SerializableDictionary<string, bool>();

		// For temporary quest related gameobjects.
		[NonSerialized, XmlIgnoreAttribute]
		public Dictionary<string, GameObject> gameObjectStore = new Dictionary<string, GameObject>();

		[Inspect]
		public string focusedQuestUniqueName;
		

















		public StateData() { }
	}
}