using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AlpacaCommon;
using Newtonsoft.Json;

namespace GameServerNet
{
    public class UdpServer
    {
        UdpClient _udpClient;

        public void Start()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 20547);
            _udpClient = new UdpClient(ipEndPoint);

            Task.Run(async () =>
            {
                Console.WriteLine(nameof(Start));
                while (true)
                {
                    try
                    {
                        while (true)
                        {
                            Console.WriteLine("Starting Receive...");
                            var udpReceiveResult = await _udpClient.ReceiveAsync();

                            OnMessageReceived(udpReceiveResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex);
                    }
                }
            });
        }

        void OnMessageReceived(UdpReceiveResult result)
        {
            var resultString = Encoding.UTF8.GetString(result.Buffer);
            var udpMessage = JsonConvert.DeserializeObject<UdpMessage>(resultString);
            var receivedMessage = new ReceivedMessage(result, udpMessage);
            var matchedClient = Program.ConnectedClients.FirstOrDefault(x => 
                x.Value.IPEndPoint.Address.ToString() == receivedMessage.Result.RemoteEndPoint.Address.ToString()
                && x.Value.IPEndPoint.Port == receivedMessage.Result.RemoteEndPoint.Port).Value;

            if (matchedClient != null)
            {
                // connected client
                ProcessConectedClientMessage(new ReceivedClientMessage(receivedMessage, matchedClient));
            }
            else
            {
                // not connected client
                ProcessUnkownClientMessage(receivedMessage);
            }
        }

        void ProcessConectedClientMessage(ReceivedClientMessage message)
        {
            switch (message.Message.Message.Event)
            {
                case "position":
                    // Send position to all other clients if any
                    SendPositionToOtherClients(message);
                    break;
                default:
                    // TODO Respond with bad request event
                    break;
            }
        }

        void SendPositionToOtherClients(ReceivedClientMessage message)
        {
            foreach (var client in Program.ConnectedClients.Values)
            {
                if (client == message.Client) continue;

                SendPosition(message, client);
            }
        }

        private void SendNewPlayerToOtherClients(GameClient gameClient)
        {
            foreach (var client in Program.ConnectedClients.Values)
            {
                if (client == gameClient) continue;

                SendMessageToClient(new UdpMessage("newplayer", gameClient.Id), client.IPEndPoint);
            }
        }

        void SendPosition(ReceivedClientMessage message, GameClient client)
        {
            SendMessageToClient(new UdpMessage("position", message.Message.Message.Data), client.IPEndPoint);
        }

        void ProcessUnkownClientMessage(ReceivedMessage message)
        {
            switch (message.Message.Event)
            {
                case "natpunch":
                    SendNatPunch(message);
                    break;
                case "connect":
                    OnConnectRequest(message);
                    break;
                default:
                    // TODO Respond with bad request event
                    break;
            }
        }

        void SendNatPunch(ReceivedMessage message)
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

        void OnConnectRequest(ReceivedMessage message)
        {
            var netId = new Guid(message.Message.Data.ToString());

            var gameClient = new GameClient(netId, message.Result.RemoteEndPoint);

            Program.ConnectedClients[netId] = gameClient;

            SendConnected(gameClient);
            SendNewPlayerToOtherClients(gameClient);
        }

        void SendConnected(GameClient client)
        {
            SendMessageToClient(new UdpMessage("connected"), client.IPEndPoint);
        }

        void SendBadMessage(GameClient client)
        {
            SendMessageToClient(new UdpMessage("badmessage"), client.IPEndPoint);
        }

        void SendMessageToClient(UdpMessage udpMessage, IPEndPoint destination)
        {
            var payloadJson = JsonConvert.SerializeObject(udpMessage);
            var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
            _udpClient.SendAsync(payloadBytes, payloadBytes.Length, destination);
            System.Console.WriteLine("Sent: " + payloadJson);
        }
    }
}