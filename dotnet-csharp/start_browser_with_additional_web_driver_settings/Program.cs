using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace StartBrowserWithAdditionalWebDriverSettings
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
            // Choose one of the Chrome BaseProfiles
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Provide additional settings for the webdriver when starting the browser
            await client.StartProfileWithWebDriverSettingsAsync(profile.Id, new WebDriverSettings
            {
                Arguments = new List<string> { "mute-audio" },
                Preferences = new List<Preference>
                {
                    new Preference("profile.managed_default_content_settings.images", 2),
                },
                AdditionalOptions = new List<Preference>
                {
                    new Preference("pageLoadStrategy", "eager"),
                }
            });

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}
