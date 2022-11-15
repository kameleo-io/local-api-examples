using System;
using System.IO;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace ProfileSaveLoad
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            // Search a Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync();

            // Create a new profile with recommended settings
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);

            // save the profile to a given path
            var result = await client.SaveProfileAsync(profile.Id, new SaveProfileRequest(Path.Combine(Environment.CurrentDirectory,"test.kameleo")));
            Console.WriteLine("Profile has been saved to " + result.LastKnownPath);

            // You have to delete this profile if you want to load back
            await client.DeleteProfileAsync(profile.Id);

            // load the profile from the given url
            profile = await client.LoadProfileAsync(new LoadProfileRequest(Path.Combine(Environment.CurrentDirectory, "test.kameleo")));

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}