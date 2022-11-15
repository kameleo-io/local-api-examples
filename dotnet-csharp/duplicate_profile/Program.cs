using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace DuplicateProfile
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            // Search Chrome Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

            // Create a new profile with recommended settings
            // Choose one of the BaseProfiles
            // You can setup here all of the profile options
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .Build();
            
            var profile = await client.CreateProfileAsync(createProfileRequest);

            // The profile needs to be stared before it can be duplicated
            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);

            // The duplicated profile is in the memory only and will be deleted when the Kameleo.CLI is closed unless you save it.
            var duplicatedProfile = await client.DuplicateProfileAsync(profile.Id);
            Console.WriteLine($"Profile '{duplicatedProfile.Name}' is just created.");
        }
    }
}