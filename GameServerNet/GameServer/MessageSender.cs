using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AlpacaCommon;
using BundtCommon;
using GameServerNet.GameServer;
using Newtonsoft.Json;

namespace GameServerNet
{
    class MessageSender
    {
        UdpClient _udpClient;
        MyLogger _logger = new MyLogger(nameof(MessageSender));

        public MessageSender(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        public void SendNatPunch(ReceivedMessage message)
        {
            var natPunchPayload = new UdpMessage("natpunchresponse")
            {
                Data = new NatPunchResponse
                {
                    PublicIpAddress = message.Result.RemoteEndPoint.Address.ToString(),
                    PublicPort = message.Result.RemoteEndPoint.Port
                }
            };

            SendMessageToClient(natPunchPayload, message.Result.RemoteEndPoint);
        }

        public void SendConnected(GameClient client)
        {
            SendMessageToClient(client.IPEndPoint, new UdpMessage("connected"));

            foreach (var item in SmartGameServer._netObjects.Values)
            {
                SendNewNetObjToClient(item, client.IPEndPoint);
            }
        }

        public void SendBadMessage(IPEndPoint ipEndpoint)
        {
            SendMessageToClient(new UdpMessage("badmessage"), ipEndpoint);
        }

        public void SendPositionToOtherClients(ReceivedClientMessage message)
        {
            SendMessageToOtherClients(new UdpMessage("position", message.Message.Message.Data), message.Client);
        }

        public void SendNewPlayerToOtherClients(GameClient gameClient)
        {
            SendMessageToOtherClients(new UdpMessage("newplayer", gameClient.Id), gameClient);
        }

        public void SendPlayerDisconnectedToAllClients(GameClient disconnectClient)
        {
            SendMessageToAllClients(new UdpMessage("playerdisconnected", disconnectClient.Id));
        }

        public void SendNewNetObjToOtherClients(GameClient client, NetObj newNetObj)
        {
            SendMessageToOtherClients(new UdpMessage("newnetobj", newNetObj), client);
        }

        public void SendDestroyNetObjToOtherClients(GameClient client, Guid destroyedNetObjGuid)
        {
            SendMessageToOtherClients(new UdpMessage("destroynetobj", destroyedNetObjGuid), client);
        }

        void SendMessageToOtherClients(UdpMessage message, GameClient ignoredClient)
        {
            foreach (var client in SmartGameServer._connectedClients.Clients)
            {
                if (client == ignoredClient) continue;
                SendMessageToClient(message, client.IPEndPoint);
            }
        }

        void SendMessageToAllClients(UdpMessage message)
        {
            foreach (var client in SmartGameServer._connectedClients.Clients)
            {
                SendMessageToClient(message, client.IPEndPoint);
            }
        }

        void SendNewNetObjToClient(NetObj item, IPEndPoint iPEndPoint)
        {
            SendMessageToClient(new UdpMessage("newnetobj", item), iPEndPoint);
        }

        void SendMessageToClient(IPEndPoint destination, UdpMessage udpMessage)
        {
            SendMessageToClient(udpMessage, destination);
        }

        void SendMessageToClient(UdpMessage udpMessage, IPEndPoint destination)
        {
            var payloadJson = JsonConvert.SerializeObject(udpMessage);
            var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
            try
            {
                _udpClient.SendAsync(payloadBytes, payloadBytes.Length, destination);
            }
            catch (Exception ex)
            {
                _logger.LogInfo(ex);
            }
        }
    }
}