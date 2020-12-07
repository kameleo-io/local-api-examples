using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace CreateProfile
{
    class Program
    {
        static async Task Main()
        {
            var client = new KameleoLocalApiClient();
            client.SetRetryPolicy(null);

            // Search Chrome Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

            // Create a new profile with recommended settings
            // Choose one of the Chrome BaseProfiles
            // You can setup here all of the profile options like Webgl
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetWebgl("noise", new WebglSpoofingOptions("Google Inc.", "ANGLE (Intel(R) HD Graphics 630 Direct3D11 vs_5_0 ps_5_0)"))
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