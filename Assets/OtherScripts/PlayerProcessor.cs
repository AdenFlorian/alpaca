using System;
using System.Collections;
using System.Collections.Generic;
using AlpacaCommon;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerProcessor : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject ZombiePrefab;
    Dictionary<Guid, NetObjGene> _otherNetObjs = new Dictionary<Guid, NetObjGene>();

    public bool isPlayer = false;

	void Start()
    {
        GameClient.Instance.Connected += OnConnected;
        GameClient.Instance.PositionUpdate += OnPositionUpdated;
        GameClient.Instance.NewPlayer += OnNewPlayer;
        GameClient.Instance.PlayerDisconnected += OnPlayerDisconnect;
        GameClient.Instance.NewNetObj += OnNewNetObj;
        GameClient.Instance.NetObjDestroyed += OnNetObjDestroyed;
	}

    void OnNetObjDestroyed(Guid destroyedNetObjGuid)
    {
        Destroy(_otherNetObjs[destroyedNetObjGuid].gameObject);
        _otherNetObjs.Remove(destroyedNetObjGuid);
    }

    void Update()
    {
		
	}

    void OnConnected()
    {
        MyLogger.LogInfo("Connected!");

        if (isPlayer)
        {
            var go = Instantiate(PlayerPrefab, Vector3.right * 200, Quaternion.identity);
            var newPlayer = go.GetComponent<Player>();
            newPlayer.NetObjGene.IsLocalPlayer = true;
            newPlayer.NetObjGene.OfflineMode = false;
            newPlayer.NetObjGene.NetObj = new NetObj { Id = Guid.NewGuid(), GameClientId = GameClient.Instance.Id, Type = NetObjType.Player };

            GameClient.Instance.SendNetObjCreate(newPlayer.NetObjGene.NetObj);
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                var go = Instantiate(ZombiePrefab, Vector3.right * 200 + new Vector3(UnityEngine.Random.Range(4, 20), UnityEngine.Random.Range(4, 20), UnityEngine.Random.Range(4, 20)), Quaternion.identity);
                var newZombie = go.GetComponent<Zombie>();
                newZombie.NetObjGene.IsLocalPlayer = true;
                newZombie.NetObjGene.OfflineMode = false;
                newZombie.NetObjGene.NetObj = new NetObj { Id = Guid.NewGuid(), GameClientId = GameClient.Instance.Id, Type = NetObjType.Zombie };

                GameClient.Instance.SendNetObjCreate(newZombie.NetObjGene.NetObj);
            }
        }

        //InstantiateExistingNetObjects(payload.ExistingNetObjects);
    }

    void InstantiateExistingNetObjects(IEnumerable<NetObj> existingNetObjects)
    {
        foreach (var netObj in existingNetObjects)
        {
            InstantiateOtherNetObj(netObj);
        }
    }

    void OnPositionUpdated(PositionUpdate update)
    {
        _otherNetObjs[update.Id].transform.position = new Vector3(update.X, update.Y, update.Z);
        _otherNetObjs[update.Id].transform.rotation = Quaternion.Euler(new Vector3(update.RotX, update.RotY, update.RotZ));
    }

    void OnNewPlayer(Guid newPlayerGuid)
    {
    }

    void OnNewNetObj(NetObj newNetObj)
    {
        InstantiateOtherNetObj(newNetObj);
    }

    void InstantiateOtherNetObj(NetObj otherNetObj)
    {
        GameObject prefab;

        switch (otherNetObj.Type)
        {
            case NetObjType.Player:
                prefab = PlayerPrefab;
                break;
            case NetObjType.Zombie:
                prefab = ZombiePrefab;
                break;
            default: throw new Exception("bad netobj type");
        }

        var go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        var netObjGene = go.GetComponent<NetObjGene>();
        netObjGene.NetObj = otherNetObj;
        netObjGene.IsLocalPlayer = false;
        netObjGene.OfflineMode = true;
        var camera = go.GetComponentInChildren<Camera>();
        if (camera != null) camera.enabled = false;
        var listener = go.GetComponentInChildren<AudioListener>();
        if (listener != null) listener.enabled = false;
        go.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        _otherNetObjs[otherNetObj.Id] = netObjGene;
    }

    void OnPlayerDisconnect(Guid disconnectedPlayerGuid)
    {
        MyLogger.LogInfo("Player disconnected: " + disconnectedPlayerGuid);
    }
}
