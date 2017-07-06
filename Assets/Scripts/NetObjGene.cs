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
        GameClient.I.SendPosition(NetObj.Id, transform);
        _positionLastSentTime = DateTime.Now;
    }

    public void OnPositionUpdatedFromNetwork(PositionUpdate update)
    {
        if (IsLocalPlayer)
        {
            MyLogger.LogWarning("Received position update from server for local player: " + update.Id);
        }
        else
        {
            transform.position = new Vector3(update.X, update.Y, update.Z);
            transform.rotation = Quaternion.Euler(new Vector3(update.RotX, update.RotY, update.RotZ));
        }
    }
}
