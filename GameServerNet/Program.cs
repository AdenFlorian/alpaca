using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AlpacaCommon;
using GameServerNet.HttpApi;
using Microsoft.AspNetCore.Hosting;

namespace GameServerNet
{
    class Program
    {
        static UdpServer _udpServer;

        static void Main(string[] args)
        {
            Console.WriteLine(nameof(Main));

            _udpServer = new UdpServer();
            _udpServer.Start();

            HttpApiStarter.StartHttpApiAsync().Wait();
        }
    }
}
