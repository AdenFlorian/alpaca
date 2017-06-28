using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVerticalLook : MonoBehaviour
{
	public float Max;
	public float Min;

	float VerticalRotation;

	void Start()
	{
		var netObjGene = GetComponent<NetObjGene>();
		if (netObjGene != null && netObjGene.IsLocalPlayer == false)
		{
			enabled = false;
		}
	}
	
	void Update()
	{
		VerticalRotation += -Input.GetAxis("Mouse Y");

		VerticalRotation = Mathf.Max(-Max, Mathf.Min(-Min, VerticalRotation));

		transform.localEulerAngles = new Vector3(VerticalRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}
}
