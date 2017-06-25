using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GameServerNet
{
    class Program
    {
        static ConcurrentQueue<string> _receivedMessages = new ConcurrentQueue<string>();

        public static ConcurrentDictionary<Guid, GameClient> ConnectedClients = new ConcurrentDictionary<Guid, GameClient>();

        static void Main(string[] args)
        {
            Console.WriteLine(nameof(Main));

            var udpServer = new UdpServer();
            udpServer.Start();

            StartUdpSender();

            StartHttpApiAsync().Wait();
        }

        private static void StartUdpSender()
        {
            Task.Run(async () =>
            {
                Console.WriteLine(nameof(StartUdpSender));
                while (true)
                {
                    while (_receivedMessages.Count == 0) await Task.Delay(10);

                    string result;
                    if (_receivedMessages.TryDequeue(out result) == false) continue;
                }
            });
        }

        private static async Task StartHttpApiAsync()
        {
            await Task.Run(() =>
            {
                Console.WriteLine(nameof(StartHttpApiAsync));

                var builder = new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseKestrel()
                    .UseUrls("http://localhost:5000");

                builder.Build().Run();
            });
        }
    }
}
