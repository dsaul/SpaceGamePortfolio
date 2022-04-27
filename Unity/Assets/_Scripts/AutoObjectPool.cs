using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoObjectPool<T,ARG1>
{
	public readonly Func<ARG1,T> d_ObjectGenerator;
	public readonly Action<T> d_ObjectCleanup;

	public readonly int m_PreloadTarget;

	Queue<T> m_Objects = new Queue<T>();

	public AutoObjectPool(Func<ARG1, T> objectGenerator, Action<T> objectCleanup, int preloadTarget = 20)
	{
		if (null == objectGenerator)
			throw new ArgumentNullException("objectGenerator");
		if (null == objectCleanup)
			throw new ArgumentNullException("objectCleanup");

		d_ObjectGenerator = objectGenerator;
		d_ObjectCleanup = objectCleanup;
		m_PreloadTarget = preloadTarget;
	}

	public void PreloadObjects(ARG1 a1)
	{
		while (m_Objects.Count < m_PreloadTarget)
		{
			m_Objects.Enqueue(d_ObjectGenerator(a1));
		}
	}

	public T Get(ARG1 a1)
	{
		if (0 == m_Objects.Count)
		{
			//Debug.LogWarning("Pool of type " + typeof(T).ToString() + "is empty");
			return d_ObjectGenerator(a1);
		}
		else
		{
			return m_Objects.Dequeue();
		}
	}

	public void Abandon(T obj)
	{
		d_ObjectCleanup(obj);
		m_Objects.Enqueue(obj);
	}
}