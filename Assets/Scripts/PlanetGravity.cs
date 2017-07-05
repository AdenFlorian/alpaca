using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetObjGene))]
public class PlanetGravity : MonoBehaviour
{
    public float gravity;
	NetObjGene _netObjGene;
	Rigidbody _rigidbody;

	// Use this for initialization
	void Start () {
		_netObjGene = GetComponent<NetObjGene>();
		_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if (_netObjGene.IsLocalPlayer)
        {
            var fromMeToOrigin = (Vector3.zero - transform.position).normalized;

            var gravityForce = fromMeToOrigin * gravity * Time.fixedDeltaTime;

            _rigidbody.AddForce(gravityForce, ForceMode.VelocityChange);
        }
    }
}
