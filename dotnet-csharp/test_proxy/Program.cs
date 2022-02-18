using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace TestProxy
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            var result = await client.TestProxyAsync(new TestProxyRequest("http", new Server("<host>", 1080, "<userId>", "<userSecret>")));

            Console.WriteLine("Test proxy result: isValid=" + result.IsValidProxy + ", message=" + result.Message);
        }
    }
}