using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class FormationSquareish
{
	private static Dictionary<float, Dictionary<int, List<Vector3>>> m_Cache = null;
	private static Dictionary<float, Dictionary<int, List<Vector3>>> Cache
	{
		get
		{
			if (null == m_Cache)
			{
				m_Cache = new Dictionary<float, Dictionary<int, List<Vector3>>>();
			}
			return m_Cache;
		}
	}


	public static List<Vector3> Get(float spacing, int unitCount)
	{
		Dictionary<int, List<Vector3>> spacingCache = null;
		if (false == Cache.TryGetValue(spacing, out spacingCache))
		{
			spacingCache = new Dictionary<int, List<Vector3>>();
			Cache[spacing] = spacingCache;
		}

		List<Vector3> list;
		if (false == spacingCache.TryGetValue(unitCount, out list))
		{
			Generate(out list, spacing, unitCount);
			spacingCache[unitCount] = list;
		}

		return list;

	}

	private static void Generate(out List<Vector3> list, float spacing, int unitCount)
	{
		List<Vector3> points = new List<Vector3>();

		int objectsPerSide = Mathf.FloorToInt(Mathf.Sqrt(unitCount));
		int remainderObjects = unitCount - (objectsPerSide * objectsPerSide);

		// All the points that can be in the circle.
		int z = 0;
		for (; z > -objectsPerSide; z--)
			for (int x = 0; x > -objectsPerSide; x--)
				points.Add(new Vector3((x * spacing) + ((objectsPerSide - 1) * spacing / 2), 0, z * spacing));

		float remainderWidth = (remainderObjects - 1) * spacing;

		for (int i = 0; i < remainderObjects; i++)
			points.Add(new Vector3((i * spacing) - (remainderWidth / 2), 0, z * spacing)); // temp

		list = points;
	}
}