using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePlane : MonoBehaviour
{
    public GameObject HighlightMesh;
    public NetObjGene NetObjGene;
    public Transform CameraRig;
    public float Speed;
    public float gravity;
    public float TorqueForce;

    Rigidbody _rigidbody;

    public bool _disableInput = true;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HighlightMesh.SetActive(false);
    }

    void FixedUpdate()
    {
        if (NetObjGene.IsLocalPlayer)
        {
            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;

            var moveVector = new Vector3();

            if (_disableInput == false)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    moveVector += transform.forward * Speed;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveVector += -transform.forward * Speed;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    moveVector += transform.up * Speed / 1.5f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    _rigidbody.AddTorque(transform.up * -TorqueForce);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    _rigidbody.AddTorque(transform.up * TorqueForce);
                }

                _rigidbody.AddTorque(transform.right * TorqueForce * -Input.GetAxis("Mouse Y"));
                _rigidbody.AddTorque(transform.forward * TorqueForce * -Input.GetAxis("Mouse X"));
            }

            var moveForce = moveVector * Time.fixedDeltaTime;
            var gravityForce = fromMeToOrigin * gravity * Time.fixedDeltaTime;

            _rigidbody.AddForce(moveForce + gravityForce, ForceMode.VelocityChange);
        }
    }

    public void OnPointedAt()
    {
        HighlightMesh.SetActive(true);
    }
}
