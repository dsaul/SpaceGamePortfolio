using System;
using UnityEngine;
[AddComponentMenu("Camera-Control/Mouse Orbit")]
[Serializable]
public class MouseOrbit : MonoBehaviour
{
	public float distance = 10f;
	public Transform target;
	public float xSpeed = 250f;
	public int yMaxLimit = 80;
	public int yMinLimit = -20;
	public float ySpeed = 120f;

	private float x;
	private float y;
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < (float)-360)
		{
			angle += (float)360;
		}
		if (angle > (float)360)
		{
			angle -= (float)360;
		}
		return Mathf.Clamp(angle, min, max);
	}

	void LateUpdate()
	{
		if (this.target)
		{
			this.x += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
			this.y -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
			this.y = MouseOrbit.ClampAngle(this.y, (float)this.yMinLimit, (float)this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.y, this.x, (float)0);
			Vector3 position = rotation * new Vector3((float)0, (float)0, -this.distance) + this.target.position;
			this.transform.rotation = rotation;
			this.transform.position = position;
		}
	}

	void Start()
	{
		Vector3 eulerAngles = this.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		// Make the rigid body not change rotation
		if (this.GetComponent<Rigidbody>())
		{
			this.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}
}


