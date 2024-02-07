using System;
using System.IO;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace ProfileExportLoad
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
            var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop");

            // Create a new profile with recommended settings
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // export the profile to a given path
            var exportPath = Path.Combine(Environment.CurrentDirectory, "test.kameleo");
            var result = await client.ExportProfileAsync(profile.Id, new ExportProfileRequest(exportPath));
            Console.WriteLine("Profile has been exported to " + exportPath);

            // You have to delete this profile if you want to import back
            await client.DeleteProfileAsync(profile.Id);

            // import the profile from the given url
            profile = await client.ImportProfileAsync(new ImportProfileRequest(Path.Combine(Environment.CurrentDirectory, "test.kameleo")));

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 10 seconds
            await Task.Delay(10_000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}