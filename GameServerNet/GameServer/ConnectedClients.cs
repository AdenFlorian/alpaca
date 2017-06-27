using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GameServerNet
{
    class ConnectedClients
    {
        static ConcurrentDictionary<Guid, GameClient> _connectedClients = new ConcurrentDictionary<Guid, GameClient>();

        public IEnumerable<GameClient> Clients => _connectedClients.Values;

        public GameClient GetByIPEndpoint(IPEndPoint remoteEndPoint)
        {
            return _connectedClients.FirstOrDefault(x =>
                x.Value.IPEndPoint.Address.ToString() == remoteEndPoint.Address.ToString()
                && x.Value.IPEndPoint.Port == remoteEndPoint.Port).Value;
        }

        public void AddOrUpdateByGuid(Guid clientGuid, GameClient newClient)
        {
            _connectedClients[clientGuid] = newClient;
        }

        public IEnumerable<Guid> GetOtherClientIds(GameClient client)
        {
            return _connectedClients.Where(x => x.Key != client.Id).Select(x => x.Value.Id);
        }

        public void KickClient(GameClient clientToKick)
        {
            GameClient removedClient;
            _connectedClients.TryRemove(clientToKick.Id, out removedClient);
            if (removedClient == null)
            {
                throw new Exception("Failed to remove client " + clientToKick.Id);
            }
            System.Console.WriteLine("Kicked client " + removedClient.Id);
        }
    }
}