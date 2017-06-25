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

public class Client : MonoBehaviour
{
    public static Client Instance;
	public GameObject PlayerPrefab;

	public bool IsConnected {get; private set;}
    bool _isPlayerSpawned = false;

	UdpClient _client = new UdpClient();

	ConcurrentQueue<string> _inboundMessageQueue = new ConcurrentQueue<string>();

    Guid PlayerNetId;

    Dictionary<Guid, Player> _otherPlayers = new Dictionary<Guid, Player>();

    void Awake()
    {
        Instance = this;
    }

	void Start()
	{
        Connect();
	}

	void Update()
	{
        if (_isPlayerSpawned == false && IsConnected == true)
        {
            var go = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<Player>().NetId = PlayerNetId;
            go.GetComponent<Player>().IsLocalPlayer = true;
            _isPlayerSpawned = true;
        }

		while (_inboundMessageQueue.Count > 0)
		{
            string result;
            _inboundMessageQueue.TryDequeue(out result);
			var message = JsonConvert.DeserializeObject<UdpMessage>(result);

			switch (message.Event)
			{
                case "position":
                    var position = JsonConvert.DeserializeObject<PositionUpdate>(message.Data.ToString());
                    _otherPlayers[position.Id].transform.position = new Vector3(position.X, position.Y, position.Z);
                    break;
                case "newplayer":
					var go = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
                    Debug.Log("result: " + result);
                    Debug.Log("message: " + message);
                    Debug.Log("message.Data: " + message.Data);
                    var newPlayerGuid = new Guid(message.Data.ToString());
                    var newPlayer = go.GetComponent<Player>();
                    newPlayer.NetId = newPlayerGuid;
                    go.GetComponentInChildren<Camera>().enabled = false;
                    _otherPlayers[newPlayerGuid] = newPlayer;
                    break;
				default:
					break;
			}
		}
	}

	void Connect()
    {
        Task.Run(async () => {

            SendNatPunch();

            await ReceiveAsync();

            SendConnect();

            var msg = await ReceiveAsync();
            Debug.Log("msg: " + msg);

            var message = JsonConvert.DeserializeObject<UdpMessage>(msg);

            Debug.Log("message.Event: " + message.Event);

            if (message.Event == "connected")
            {
                IsConnected = true;
                Debug.Log("connected! starting receive loop");
                StartReceiveLoop();
            }
        });
    }

    void StartReceiveLoop()
    {
        Task.Run(async () =>
        {
            while (true)
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

	async Task<string> ReceiveAsync()
	{
        var receiveResult = await _client.ReceiveAsync();
        var receivedMsg = Encoding.UTF8.GetString(receiveResult.Buffer);
        Debug.Log("received: " + receivedMsg);
        return receivedMsg;
	}

    void SendNatPunch()
	{
        SendMessageToServer("{event: 'natpunch', data: '" + DateTime.Now.ToString("o") + "'}");
	}
	
	void SendConnect()
	{
		PlayerNetId = Guid.NewGuid();
        SendMessageToServer("{event: 'connect', data: '" + PlayerNetId + "'}");
	}

	public void SendMessageToServer(string message)
	{
        var msgBytes = Encoding.UTF8.GetBytes(message);

        _client.SendAsync(msgBytes, msgBytes.Length, "localhost", 20547);

        Debug.Log("msg sent: " + message);
	}
}
