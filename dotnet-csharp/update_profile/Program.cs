using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace UpdateProfile
{
    class Program
    {
        static async Task Main()
        {
            var client = new KameleoLocalApiClient();
            client.SetRetryPolicy(null);

            // Search a Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync();

            // Create a new profile with recommended settings
            // Choose one of the BaseProfiles
            // You can setup here all of the profile options
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Change every properties what you want to update
            // Others should be the same
            var updateRequestBody = new UpdateProfileRequest(profile)
            {
                StartPage = "https://www.google.com", Canvas = "noise"
            };

            // Send the update request and the response will be your new profile
            profile = await client.UpdateProfileAsync(profile.Id, updateRequestBody);

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);

        }
    }
}