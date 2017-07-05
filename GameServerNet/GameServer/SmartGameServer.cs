using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlpacaCommon;
using BundtCommon;

namespace GameServerNet.GameServer
{
    class SmartGameServer
    {
        public static ConcurrentDictionary<Guid, NetObj> _netObjects = new ConcurrentDictionary<Guid, NetObj>();
        public static ConnectedClients _connectedClients = new ConnectedClients();
        
        UdpServer _udpServer;
        MyLogger _logger = new MyLogger(nameof(SmartGameServer));

        public SmartGameServer()
        {
            _udpServer = new UdpServer();

            _udpServer.NetObjCreated += OnNetObjCreated;
            _udpServer.PositionUpdated += OnPositionUpdated;
            _udpServer.Connect += OnConnect;
            _udpServer.NatPunch += OnNatPunch;
            _udpServer.OwnershipRequested += OnOwnershipRequested;
            _udpServer.BadConnectedClientMessage += OnBadConnectedClientMessage;
        }

        public void Start()
        {
            _udpServer.Start();
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
                        foreach (var client in SmartGameServer._connectedClients.Clients)
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

        void OnNetObjCreated(GameClient sender, NetObj newNetObj)
        {
            _netObjects[newNetObj.Id] = newNetObj;
            _udpServer.MessageSender.SendNewNetObjToOtherClients(sender, newNetObj);
        }

        void OnPositionUpdated(ReceivedClientMessage message)
        {
            _udpServer.MessageSender.SendPositionToOtherClients(message);
        }

        void OnNatPunch(ReceivedMessage message)
        {
            _logger.LogInfo("natpunch requested from " + message.Result.RemoteEndPoint);
            _udpServer.MessageSender.SendNatPunch(message);
        }

        void OnConnect(ReceivedMessage message)
        {
            _logger.LogInfo("client request to connect from " + message.Result.RemoteEndPoint);

            var netId = new Guid(message.Message.Data.ToString());
            var gameClient = new GameClient(netId, message.Result.RemoteEndPoint);

            _connectedClients.AddOrUpdateByGuid(netId, gameClient);

            _udpServer.MessageSender.SendConnected(gameClient);
            _udpServer.MessageSender.SendNewPlayerToOtherClients(gameClient);
            _logger.LogInfo("Client connected: " + gameClient.Id);
        }

        void OnOwnershipRequested()
        {

        }

        void OnBadConnectedClientMessage(ReceivedClientMessage message)
        {
            _logger.LogInfo("client sent bad message: " + message.Client.Id);
            _udpServer.MessageSender.SendBadMessage(message.Client.IPEndPoint);
        }

        void OnBadUnkownClientMessage(ReceivedMessage message)
        {
            _logger.LogInfo("client sent bad message from " + message.Result.RemoteEndPoint);
            _udpServer.MessageSender.SendBadMessage(message.Result.RemoteEndPoint);
        }

        public void KickClient(GameClient clientToKick)
        {
            _connectedClients.KickClient(clientToKick);
            foreach (var netObj in _netObjects.Values.Where(x => x.GameClientId == clientToKick.Id))
            {
                _udpServer.MessageSender.SendDestroyNetObjToOtherClients(clientToKick, netObj.Id);
            }
            NetObj removedNetObj = null;
            _netObjects.Where(x => x.Value.GameClientId == clientToKick.Id).ToList().ForEach(x => _netObjects.Remove(x.Key, out removedNetObj));
            if (removedNetObj == null)
            {
                throw new Exception("Failed to remove netobj");
            }
            _udpServer.MessageSender.SendPlayerDisconnectedToAllClients(clientToKick);
        }
    }
}