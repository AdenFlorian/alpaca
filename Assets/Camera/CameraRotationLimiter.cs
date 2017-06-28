using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationLimiter : MonoBehaviour
{
	void Start()
	{
		
	}
	
	void Update()
	{
		if (transform.localRotation.eulerAngles.x > 30)
		{
			transform.localRotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
		}
	}
}
