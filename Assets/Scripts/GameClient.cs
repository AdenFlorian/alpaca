using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AlpacaCommon;
using Newtonsoft.Json;
using UnityEngine;

public enum GameServerLocation
{
    LocalHost,
    AlpacaTest
}

public class GameClient : MonoBehaviour
{
    public static GameClient I;

    public GameServerLocation GameServerLocation;

    public readonly Guid Id = Guid.NewGuid();

    public event Action Connected;
    public event Action<PositionUpdate> PositionUpdate;
    public event Action<Guid> NewPlayer;
    public event Action<Guid> PlayerDisconnected;
    public event Action<NetObj> NewNetObj;
    public event Action<Guid> NetObjDestroyed;
    public event Action<Guid> OwnerChanged;

    UdpClient _udpClient;
	ConcurrentQueue<string> _inboundMessageQueue = new ConcurrentQueue<string>();

    int _messagesSentInLastSecond = 0;
    bool _isDestroyed;

    string _serverHostName;
    int _serverPort;

    void Awake()
    {
        I = this;
        SetupServerInfo();
        _udpClient = new UdpClient(_serverHostName, _serverPort);
    }

    void SetupServerInfo()
    {
        switch (GameServerLocation)
        {
            case GameServerLocation.LocalHost:
                _serverHostName = "localhost";
                _serverPort = 20547;
                break;
            case GameServerLocation.AlpacaTest:
                _serverHostName = "alpaca.AdenFlorian.com";
                _serverPort = 20547;
                break;
        }

        Debug.Log("Game server location set to " + _serverHostName + ":" + _serverPort);
    }

	void Start()
	{
        StartDiagnosticLoop();
        StartReceiveLoop();
        SendConnect();
	}

    void OnDestroy()
    {
        _isDestroyed = true;
    }

    void StartDiagnosticLoop()
    {
        Task.Run(async () =>
        {
            while (_isDestroyed == false)
            {
                await Task.Delay(1000);
                Debug.Log(_messagesSentInLastSecond + " net messages per second");
                _messagesSentInLastSecond = 0;
            }
        });
    }

    void StartReceiveLoop()
    {
        Task.Run(async () =>
        {
            while (_isDestroyed == false)
            {
                try
                {
                    var message = await ReceiveAsync();
                    _inboundMessageQueue.Enqueue(message);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        });
    }

	void Update()
	{
        HandleInboundMessages();
	}

    void HandleInboundMessages()
    {
        while (_inboundMessageQueue.Count > 0)
        {
            string result;
            _inboundMessageQueue.TryDequeue(out result);
            var message = JsonConvert.DeserializeObject<UdpMessage>(result);
            HandleInboundMessage(message);
        }
    }

    void HandleInboundMessage(UdpMessage message)
    {
        switch (message.Event)
        {
            case "connected": Connected?.Invoke(); break;
            case "position": PositionUpdate?.Invoke(Deserialize<PositionUpdate>(message.Data.ToString())); break;
            case "newplayer": NewPlayer?.Invoke(new Guid(message.Data.ToString())); break;
            case "playerdisconnected": PlayerDisconnected?.Invoke(new Guid(message.Data.ToString())); break;
            case "newnetobj": NewNetObj?.Invoke(Deserialize<NetObj>(message.Data.ToString())); break;
            case "destroynetobj": NetObjDestroyed?.Invoke(new Guid(message.Data.ToString())); break;
            case "owner-changed": OwnerChanged?.Invoke(new Guid(message.Data.ToString())); break;
            default: Debug.LogError("Received invalid inbound message event: " + message.Event); break;
        }
    }

    T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore
        });
    }

	async Task<string> ReceiveAsync()
	{
        var receiveResult = await _udpClient.ReceiveAsync();
        var receivedMsg = Encoding.UTF8.GetString(receiveResult.Buffer);
        MyLogger.LogTrace("received: " + receivedMsg);
        return receivedMsg;
	}

    void SendNatPunch() => SendMessageToServer(new UdpMessage("natpunch", DateTime.Now.ToString("o")));

    void SendConnect() => SendMessageToServer(new UdpMessage("connect", Id));

    public void SendNetObjCreate(NetObj newNetObj) => SendMessageToServer(new UdpMessage("netobjcreate", newNetObj));

    public void SendPosition(Guid netObjId, Transform transformToSend)
    {
        var positionMessage = new UdpMessage("position")
        {
            Data = new PositionUpdate
            {
                Id = netObjId,
                X = transformToSend.position.x,
                Y = transformToSend.position.y,
                Z = transformToSend.position.z,
                RotX = transformToSend.rotation.eulerAngles.x,
                RotY = transformToSend.rotation.eulerAngles.y,
                RotZ = transformToSend.rotation.eulerAngles.z,
            }
        };
        SendMessageToServer(positionMessage);
    }

    public void SendOwnershipRequest(Guid netObjIdToRequest) => SendMessageToServer(new UdpMessage("request-ownership", netObjIdToRequest));

	void SendMessageToServer(object message)
	{
        var json = JsonConvert.SerializeObject(message);
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        _udpClient.SendAsync(jsonBytes, jsonBytes.Length);

        MyLogger.LogTrace("msg sent: " + json);
        _messagesSentInLastSecond++;
	}
}
