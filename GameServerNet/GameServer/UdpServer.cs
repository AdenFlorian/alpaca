using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AlpacaCommon;
using BundtCommon;
using GameServerNet.GameServer;
using Newtonsoft.Json;

namespace GameServerNet
{
    class UdpServer
    {
        // Connected client events
        public event Action<GameClient, NetObj> NetObjCreated;
        public event Action<ReceivedClientMessage> PositionUpdated;
        public event Action OwnershipRequested;
        public event Action<ReceivedClientMessage> BadConnectedClientMessage;
        
        // Unkown client events
        public event Action<ReceivedMessage> NatPunch;
        public event Action<ReceivedMessage> Connect;
        public event Action<ReceivedMessage> BadUnkownClientMessage;

        public MessageSender MessageSender;

        UdpClient _udpClient;
        MessageReceiver _messageReceiver;
        MyLogger _logger = new MyLogger(nameof(UdpServer));

        public UdpServer()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 20547);
            _udpClient = new UdpClient(ipEndPoint);
            _messageReceiver = new MessageReceiver(_udpClient);
            _messageReceiver.MessageReceived += OnMessageReceived;
            MessageSender = new MessageSender(_udpClient);
        }

        public void Start()
        {
            _messageReceiver.StartReceiving();
        }

        void OnMessageReceived(UdpReceiveResult result)
        {
            var udpMessage = ExtractUdpMessage(result);
            var receivedMessage = new ReceivedMessage(result, udpMessage);
            var matchedClient = SmartGameServer._connectedClients.GetByIPEndpoint(receivedMessage.Result.RemoteEndPoint);

            if (matchedClient != null)
            {
                ProcessConectedClientMessage(new ReceivedClientMessage(receivedMessage, matchedClient));
            }
            else
            {
                ProcessUnkownClientMessage(receivedMessage);
            }
        }

        static UdpMessage ExtractUdpMessage(UdpReceiveResult result)
        {
            try
            {
                var resultString = Encoding.UTF8.GetString(result.Buffer);
                return JsonConvert.DeserializeObject<UdpMessage>(resultString);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to extract a UdpMessage, client probably sent a bad message", ex);
            }
        }

        void ProcessConectedClientMessage(ReceivedClientMessage message)
        {
            message.Client.RefreshLastActivity();

            switch (message.Message.Message.Event)
            {
                case "position":  PositionUpdated?.Invoke(message); break;
                case "netobjcreate":
                    var newNetObj = JsonConvert.DeserializeObject<NetObj>(message.Message.Message.Data.ToString());
                    _logger.LogInfo("netobjcreate: " + message.Message.Message.Data.ToString());
                    NetObjCreated?.Invoke(message.Client, newNetObj);
                    break;
                case "request-ownership": OwnershipRequested?.Invoke(); break;
                default: BadConnectedClientMessage?.Invoke(message); break;
            }
        }

        void ProcessUnkownClientMessage(ReceivedMessage message)
        {
            switch (message.Message.Event)
            {
                case "natpunch": NatPunch?.Invoke(message); break;
                case "connect": Connect?.Invoke(message); break;
                default: BadUnkownClientMessage?.Invoke(message); break;
            }
        }
    }
}