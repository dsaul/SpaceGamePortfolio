using UnityEngine;
using System.Collections;
using SpaceInsurgency.Items;

namespace SpaceInsurgency
{
	public static class Const
	{
		public static class Tags
		{
			public const string Ignore = "Ignore!";
			public const string Player = "Player";
			public const string MovementPlane = "Movement Plane";
		}

		public static class Vector3
		{
			public static UnityEngine.Vector3 Sentinel = new UnityEngine.Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		}
		public static class Vector2
		{
			public static UnityEngine.Vector2 Sentinel = new UnityEngine.Vector2(float.MaxValue, float.MaxValue);
			public static UnityEngine.Vector2 HalfOffset = new UnityEngine.Vector2(0.5f, 0.5f);
		}
		public static class Color
		{
			public static UnityEngine.Color BlackA80 = new UnityEngine.Color(0f, 0f, 0f, 0.8f);
			public static UnityEngine.Color WhiteA80 = new UnityEngine.Color(1f, 1f, 1f, 0.8f);
			public static UnityEngine.Color WhiteA50 = new UnityEngine.Color(1f, 1f, 1f, 0.5f);
			public static UnityEngine.Color RedA80 = new UnityEngine.Color(1f, 0f, 0f, 0.8f);
			public static UnityEngine.Color CyanA80 = new UnityEngine.Color(0f, 255f, 255f, 0.8f);
			public static UnityEngine.Color YellowA80 = new UnityEngine.Color(255f, 255f, 0f, 0.8f);
		}

		public const float FormationSpacing = 3f;

		public static Base[] EmptyItemArray = new Base[0];

	}

	public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
	//public class Nothing { }
}

