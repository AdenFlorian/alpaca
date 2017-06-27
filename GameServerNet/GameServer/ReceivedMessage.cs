using System.Net.Sockets;
using AlpacaCommon;

namespace GameServerNet
{
    public class ReceivedMessage
    {
        public readonly UdpReceiveResult Result;
        public readonly UdpMessage Message;

        public ReceivedMessage(UdpReceiveResult result, UdpMessage message)
        {
            Result = result;
            Message = message;
        }
    }
}