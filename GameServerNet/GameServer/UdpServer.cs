using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AlpacaCommon;
using BundtCommon;
using Newtonsoft.Json;

namespace GameServerNet
{
    class UdpServer
    {
        public static ConnectedClients _connectedClients = new ConnectedClients();
        public static ConcurrentDictionary<Guid, NetObj> _netObjects = new ConcurrentDictionary<Guid, NetObj>();

        UdpClient _udpClient;
        MessageReceiver _messageReceiver;
        MessageSender _messageSender;

        MyLogger _logger = new MyLogger(nameof(UdpServer));

        public UdpServer()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 20547);
            _udpClient = new UdpClient(ipEndPoint);
            _messageReceiver = new MessageReceiver(_udpClient);
            _messageReceiver.MessageReceived += OnMessageReceived;
            _messageSender = new MessageSender(_udpClient);
        }

        public void Start()
        {
            _messageReceiver.StartReceiving();
            StartClientActivityMonitor();
        }

        void StartClientActivityMonitor()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(2000);
                        foreach (var client in _connectedClients.Clients)
                        {
                            if (client.LastActivity < (DateTime.Now - TimeSpan.FromSeconds(5)))
                            {
                                KickClient(client);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                }
            });
        }

        void OnMessageReceived(UdpReceiveResult result)
        {
            var resultString = Encoding.UTF8.GetString(result.Buffer);
            var udpMessage = JsonConvert.DeserializeObject<UdpMessage>(resultString);
            var receivedMessage = new ReceivedMessage(result, udpMessage);
            var matchedClient = _connectedClients.GetByIPEndpoint(receivedMessage.Result.RemoteEndPoint);

            if (matchedClient != null)
            {
                ProcessConectedClientMessage(new ReceivedClientMessage(receivedMessage, matchedClient));
            }
            else
            {
                ProcessUnkownClientMessage(receivedMessage);
            }
        }

        void ProcessConectedClientMessage(ReceivedClientMessage message)
        {
            message.Client.RefreshLastActivity();

            switch (message.Message.Message.Event)
            {
                case "position":
                    _messageSender.SendPositionToOtherClients(message);
                    break;
                case "netobjcreate":
                    var newNetObj = JsonConvert.DeserializeObject<NetObj>(message.Message.Message.Data.ToString());
                    _netObjects[newNetObj.Id] = newNetObj;
                    _messageSender.SendNewNetObjToOtherClients(message.Client, newNetObj);
                    break;
                default:
                    _logger.LogInfo("client sent bad message: " + message.Client.Id);
                    _messageSender.SendBadMessage(message.Client.IPEndPoint);
                    break;
            }
        }

        void ProcessUnkownClientMessage(ReceivedMessage message)
        {
            switch (message.Message.Event)
            {
                case "natpunch":
                    _logger.LogInfo("natpunch requested from " + message.Result.RemoteEndPoint);
                    _messageSender.SendNatPunch(message);
                    break;
                case "connect":
                    _logger.LogInfo("client request to connect from " + message.Result.RemoteEndPoint);
                    OnConnectRequest(message);
                    break;
                default:
                    _logger.LogInfo("client sent bad message from " + message.Result.RemoteEndPoint);
                    _messageSender.SendBadMessage(message.Result.RemoteEndPoint);
                    break;
            }
        }

        void OnConnectRequest(ReceivedMessage message)
        {
            var netId = new Guid(message.Message.Data.ToString());
            var gameClient = new GameClient(netId, message.Result.RemoteEndPoint);

            _connectedClients.AddOrUpdateByGuid(netId, gameClient);

            _messageSender.SendConnected(gameClient);
            _messageSender.SendNewPlayerToOtherClients(gameClient);
            _logger.LogInfo("Client connected: " + gameClient.Id);
        }

        public void KickClient(GameClient clientToKick)
        {
            _connectedClients.KickClient(clientToKick);
            foreach (var netObj in _netObjects.Values.Where(x => x.GameClientId == clientToKick.Id))
            {
                _messageSender.SendDestroyNetObjToOtherClients(clientToKick, netObj.Id);
            }
            NetObj removedNetObj = null;
            _netObjects.Where(x => x.Value.GameClientId == clientToKick.Id).ToList().ForEach(x => _netObjects.Remove(x.Key, out removedNetObj));
            if (removedNetObj == null)
            {
                throw new Exception("Failed to remove netobj");
            }
            _messageSender.SendPlayerDisconnectedToAllClients(clientToKick);
        }
    }
}