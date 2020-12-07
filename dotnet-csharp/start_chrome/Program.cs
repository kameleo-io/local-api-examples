using System.Threading.Tasks;
using Kameleo.LocalApiClient;

namespace StartChromium
{
    class Program
    {
        static async Task Main()
        {
            var client = new KameleoLocalApiClient();
            client.SetRetryPolicy(null);

            // Search Chrome Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync("desktop", null, "chrome", null);

            // Create a new profile with recommended settings
            // Choose one of the Chrome BaseProfiles
            // You can setup here all of the profile options
            // Set the launcher "chromium" for launching chromium
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetLauncher("chromium")
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