using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace StartWithProxy
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var proxyHost = Environment.GetEnvironmentVariable("PROXY_HOST") ?? "<your_proxy_host>";
            var proxyPort = Convert.ToInt32(Environment.GetEnvironmentVariable("PROXY_PORT") ?? "1080");
            var proxyUsername = Environment.GetEnvironmentVariable("PROXY_USERNAME") ?? "<your_username>";
            var proxyPassword = Environment.GetEnvironmentVariable("PROXY_PASSWORD") ?? "<your_password>";

            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            // Search Firefox Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync("desktop", null, "firefox", null);

            // Create a new profile with recommended settings
            // Choose one of the Firefox BaseProfiles
            // You can set your proxy up in the setProxy method
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .SetProxy("socks5", new Server(proxyHost, proxyPort, proxyUsername, proxyPassword))
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10_000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}