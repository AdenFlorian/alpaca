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
    public GameObject SpacePlanePrefab;
    public Transform PlayerSpawnPosition;

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

    void Update() {}

    void OnConnected()
    {
        MyLogger.LogInfo("Connected!");
        SpawnLocalStuff();
    }

    void SpawnLocalStuff()
    {
        if (isPlayer)
        {
            SpawnLocalPlayer();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnLocalZombie();
            }

            SpawnLocalSpacePlane();
        }
    }

    void SpawnLocalPlayer()
    {
        SpawnLocalNetObj(PlayerPrefab, PlayerSpawnPosition.position, NetObjType.Player);
    }

    void SpawnLocalZombie()
    {
        SpawnLocalNetObj(ZombiePrefab, PlayerSpawnPosition.position + new Vector3(UnityEngine.Random.Range(6, 30), UnityEngine.Random.Range(4, 20), UnityEngine.Random.Range(4, 20)), NetObjType.Zombie);
    }

    void SpawnLocalSpacePlane()
    {
        SpawnLocalNetObj(SpacePlanePrefab, PlayerSpawnPosition.position + new Vector3(UnityEngine.Random.Range(6, 30), UnityEngine.Random.Range(4, 20), UnityEngine.Random.Range(4, 20)), NetObjType.SpacePlane);
    }

    void SpawnLocalNetObj(GameObject prefab, Vector3 position, NetObjType type)
    {
        var go = Instantiate(prefab, position, Quaternion.identity);
        var netObjGene = go.GetComponent<NetObjGene>();
        netObjGene.IsLocalPlayer = true;
        netObjGene.OfflineMode = false;
        netObjGene.NetObj = new NetObj { Id = Guid.NewGuid(), GameClientId = GameClient.Instance.Id, Type = type };

        GameClient.Instance.SendNetObjCreate(netObjGene.NetObj);
    }

    void OnPositionUpdated(PositionUpdate update)
    {
        _otherNetObjs[update.Id].transform.position = new Vector3(update.X, update.Y, update.Z);
        _otherNetObjs[update.Id].transform.rotation = Quaternion.Euler(new Vector3(update.RotX, update.RotY, update.RotZ));
    }

    void OnNewPlayer(Guid newPlayerGuid)
    {
        Debug.Log("New player! " + newPlayerGuid);
    }

    void OnNewNetObj(NetObj newNetObj)
    {
        Debug.Log("New net obj! " + newNetObj.GameClientId + ": " + newNetObj.Id);
        InstantiateOtherNetObj(newNetObj);
    }

    void InstantiateOtherNetObj(NetObj otherNetObj)
    {
        GameObject prefab = GetPrefabFromNetObjType(otherNetObj.Type);
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

    GameObject GetPrefabFromNetObjType(NetObjType netObjType)
    {
        switch (netObjType)
        {
            case NetObjType.Player: return PlayerPrefab;
            case NetObjType.Zombie: return ZombiePrefab;
            case NetObjType.SpacePlane: return SpacePlanePrefab;
            default: throw new Exception("bad netobj type: " + netObjType);
        }
    }

    void OnPlayerDisconnect(Guid disconnectedPlayerGuid)
    {
        MyLogger.LogInfo("Player disconnected: " + disconnectedPlayerGuid);
    }
}
