using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace StartWithProxy
{
    class Program
    {
        static async Task Main()
        {
            var client = new KameleoLocalApiClient();
            client.SetRetryPolicy(null);

            // Search Firefox Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync("desktop", null, "firefox", null);

            // Create a new profile with recommended settings
            // Choose one of the Firefox BaseProfiles
            // You can set your proxy up in the setProxy method
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .SetProxy("socks5", new Server("<proxy_host>", 1080, "<username>", "<password>"))
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}