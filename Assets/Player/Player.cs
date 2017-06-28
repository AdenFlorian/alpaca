using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaCommon;
using Newtonsoft.Json;

public class Player : MonoBehaviour
{
	public float Speed;
    public float gravity;
    public float JumpForce;

    public Camera Camera;
    public NetObjGene NetObjGene;

    Rigidbody _rigidbody;

    float localYRotation;

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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rigidbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            }

            localYRotation += Input.GetAxis("Mouse X");

            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(-fromMeToOrigin);
            transform.eulerAngles += new Vector3(90, 0, 0);
            
            transform.Rotate(Vector3.up, localYRotation, Space.Self);
        }
	}

    void FixedUpdate()
    {
        if (NetObjGene.IsLocalPlayer)
        {
            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;

            var moveVector = new Vector3();

            if (Input.GetKey(KeyCode.W))
            {
                moveVector += transform.forward * Speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveVector += -transform.forward * Speed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveVector += -transform.right * Speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveVector += transform.right * Speed;
            }

            var moveForce = moveVector * Time.fixedDeltaTime;
            var gravityForce = fromMeToOrigin * gravity * Time.fixedDeltaTime;

            _rigidbody.AddForce(moveForce + gravityForce, ForceMode.VelocityChange);
        }
    }
}
