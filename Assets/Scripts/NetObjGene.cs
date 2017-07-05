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
}
