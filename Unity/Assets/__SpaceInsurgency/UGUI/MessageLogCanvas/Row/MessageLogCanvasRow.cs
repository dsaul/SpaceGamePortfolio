using UnityEngine;
using System.Collections;
using SharedCode;

public class MessageLogCanvasRow : SharedCode.Behaviours.Base
{
	public void RemovingAnimationComplete()
	{
		Debug.Log("RemovingAnimationComplete()");
		GameObject.Destroy(gameObject);
	}
}