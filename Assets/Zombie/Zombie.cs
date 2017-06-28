using System.Collections;
using System.Collections.Generic;
using AlpacaCommon;
using UnityEngine;

public class Zombie : MonoBehaviour
{
	public float Speed;
    public float gravity;
    public float JumpForce;

    public NetObjGene NetObjGene;

    Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.rotation = Random.rotation;
        transform.Translate(Random.Range(10, 50), Random.Range(10, 50), Random.Range(10, 50));
    }

    void Update()
    {
        if (NetObjGene.IsLocalPlayer)
        {
            if (Random.Range(1, 100) < 1.3f)
            {
                _rigidbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            }

            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(-fromMeToOrigin);
            transform.eulerAngles += new Vector3(90, 0, 0);
        }
    }

    void FixedUpdate()
    {
        if (NetObjGene.IsLocalPlayer)
        {
            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;

            var gravityForce = fromMeToOrigin * gravity * Time.fixedDeltaTime;

            _rigidbody.AddForce(gravityForce, ForceMode.VelocityChange);
        }
    }
}
