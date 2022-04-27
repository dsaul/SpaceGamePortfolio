using UnityEngine;
using System.Collections;

public class TrackTransformPosition : MonoBehaviour
{
	public Transform toTrack;

	public bool trackPosition = true;
	public bool trackRotation = false;

	void Update()
	{
		DoTrack();
	}

	public void DoTrack()
	{
		if (null == toTrack)
			return;
		if (true == trackPosition)
			transform.position = toTrack.transform.position;
		if (true == trackRotation)
			transform.rotation = toTrack.transform.rotation;
	}
}