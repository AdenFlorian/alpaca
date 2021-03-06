﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaCommon;
using Newtonsoft.Json;

public class Player : MonoBehaviour
{
	public float Acceleration;
    public float Gravity;
    public float JumpForce;
    public float MaxMoveVelocity;
    public float GroundedCheckRayLength;

    public Collider[] Colliders;

    public Camera Camera;
    public NetObjGene NetObjGene;

    Rigidbody _rigidbody;

    float localYRotation;
    bool _disableInput;
    bool _inPlane = true;
    SpacePlane MyPlane;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        localYRotation = transform.localEulerAngles.y;
	}

	void Update()
	{
        if (NetObjGene.IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _disableInput == false && IsGrounded())
            {
                _rigidbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            }

            localYRotation += Input.GetAxis("Mouse X");

            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(-fromMeToOrigin);
            transform.eulerAngles += new Vector3(90, 0, 0);
            
            transform.Rotate(Vector3.up, localYRotation, Space.Self);

            if (_inPlane && MyPlane != null)
            {
                transform.position = MyPlane.transform.position;
                transform.rotation = MyPlane.transform.rotation;
                Camera.transform.position = MyPlane.CameraRig.position;
                Camera.transform.rotation = MyPlane.CameraRig.rotation;
            }
        }
	}

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, GroundedCheckRayLength);
    }

    void FixedUpdate()
    {
        if (NetObjGene.IsLocalPlayer)
        {
            if (_disableInput == false)
            {
                var fromMeToOrigin = (Vector3.zero - transform.position).normalized;

                var moveVector = new Vector3();

                if (Input.GetKey(KeyCode.W))
                {
                    moveVector += transform.forward * Acceleration;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveVector += -transform.forward * Acceleration;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveVector += -transform.right * Acceleration;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveVector += transform.right * Acceleration;
                }

                var moveForce = moveVector * Time.fixedDeltaTime;

                if (_rigidbody.velocity.magnitude > MaxMoveVelocity)
                {
                    moveForce *= 0;
                }

                var gravityForce = fromMeToOrigin * Gravity * Time.fixedDeltaTime;

                _rigidbody.AddForce(moveForce + gravityForce, ForceMode.VelocityChange);
            }
        }
    }

    public void GetInPlane(SpacePlane spacePlane)
    {
        GameClient.I?.SendOwnershipRequest(spacePlane.NetObjGene.NetObj.Id);

        _rigidbody.isKinematic = true;
        transform.position = spacePlane.transform.position;
        _disableInput = true;
        _inPlane = true;
        MyPlane = spacePlane;
        foreach (var Collider in Colliders)
        {
            Collider.gameObject.SetActive(false);
        }
        spacePlane.NetObjGene.IsLocalPlayer = true;
        spacePlane.DisableInput = false;
        spacePlane.GetComponent<Rigidbody>().isKinematic = false;
        Camera.GetComponent<CameraVerticalLook>().enabled = false;
    }
}
