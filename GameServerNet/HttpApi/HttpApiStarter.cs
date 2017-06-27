using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GameServerNet.HttpApi
{
    class HttpApiStarter
    {
        public static async Task StartHttpApiAsync()
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