using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaCommon;
using Newtonsoft.Json;

public class Player : MonoBehaviour
{
	public float Speed = 0.01f;

    public Guid NetId;
    public bool IsLocalPlayer = false;

	void Start()
	{
		
	}

	void Update()
	{
        if (IsLocalPlayer)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.forward * Speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += Vector3.forward * -Speed;
            }

            if (Client.Instance.IsConnected)
            {
                SendPosition();
            }
        }
	}

    private void SendPosition()
    {
        var positionMessage = new UdpMessage("position")
        {
            Data = new PositionUpdate
            {
                Id = NetId,
                X = transform.position.x,
                Y = transform.position.y,
                Z = transform.position.z
            }
        };
        Client.Instance.SendMessageToServer(JsonConvert.SerializeObject(positionMessage));
    }
}
