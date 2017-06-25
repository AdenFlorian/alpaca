using System;
using System.Net;

namespace GameServerNet
{
    public class GameClient
    {
        public readonly Guid Id;
        public readonly IPEndPoint IPEndPoint;

        public GameClient(Guid id, IPEndPoint ipEndPoint)
        {
            Id = id;
            IPEndPoint = ipEndPoint;
        }
    }
}