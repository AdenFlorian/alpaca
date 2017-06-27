using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BundtCommon;

namespace GameServerNet
{
    public class MessageReceiver
    {
        public event Action<UdpReceiveResult> MessageReceived;

        UdpClient _udpClient;
        MyLogger _logger = new MyLogger(nameof(MessageReceiver));

        public MessageReceiver(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        public void StartReceiving()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var udpReceiveResult = await _udpClient.ReceiveAsync();
                        MessageReceived?.Invoke(udpReceiveResult);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode != SocketError.ConnectionReset)
                        {
                            _logger.LogError(ex);
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                        throw;
                    }
                }
            });
        }
    }
}