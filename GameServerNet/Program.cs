using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AlpacaCommon;
using GameServerNet.GameServer;
using GameServerNet.HttpApi;
using Microsoft.AspNetCore.Hosting;

namespace GameServerNet
{
    class Program
    {
        static SmartGameServer _smartGameServer;

        static void Main(string[] args)
        {
            Console.WriteLine(nameof(Main));

            _smartGameServer = new SmartGameServer();
            _smartGameServer.Start();

            HttpApiStarter.StartHttpApiAsync().Wait();
        }
    }
}
