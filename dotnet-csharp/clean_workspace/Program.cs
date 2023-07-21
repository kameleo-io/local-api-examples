using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;

namespace CleanWorkspace
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            var profiles = await client.ListProfilesAsync();

            foreach (var profile in profiles)
            {
                await client.DeleteProfileAsync(profile.Id);
            }

            Console.WriteLine($"{profiles.Count} profiles deleted.");
        }
    }
}