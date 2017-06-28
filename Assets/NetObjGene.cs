using System;
using System.Collections;
using System.Collections.Generic;
using AlpacaCommon;
using UnityEngine;

public class NetObjGene : MonoBehaviour
{
	public NetObj NetObj;
    public bool IsLocalPlayer = false;
    DateTime _positionLastSentTime;
    public bool OfflineMode;

	void Start()
	{
		
	}
	
	void Update()
	{
        if (IsLocalPlayer && IsItTimeToSendPosition() && OfflineMode == false)
        {
            SendPosition();
        }
	}

    bool IsItTimeToSendPosition()
    {
        if (DateTime.Now - _positionLastSentTime > TimeSpan.FromMilliseconds(40))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SendPosition()
    {
        var positionMessage = new UdpMessage("position")
        {
            Data = new PositionUpdate
            {
                Id = NetObj.Id,
                X = transform.position.x,
                Y = transform.position.y,
                Z = transform.position.z,
                RotX = transform.rotation.eulerAngles.x,
                RotY = transform.rotation.eulerAngles.y,
                RotZ = transform.rotation.eulerAngles.z,
            }
        };
        GameClient.Instance.SendMessageToServer(positionMessage);
        _positionLastSentTime = DateTime.Now;
    }
}
