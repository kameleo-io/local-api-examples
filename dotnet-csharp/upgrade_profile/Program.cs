﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace UpgradeProfile
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            // Search for a Desktop Base Profile with Windows OS and Chrome browser
            var baseProfileList = await client.SearchBaseProfilesAsync("desktop","windows","chrome");

            // Find a Base Profile with the oldest available version of chrome
            var baseProfile = baseProfileList.OrderBy(preview => preview.Browser.Major).First();

            // Create a new profile with recommended settings
            // Choose one of the Base Profiles
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfile.Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);
            
            Console.WriteLine($"Profile's browser before update is: {profile.BaseProfile.Browser.Product} {profile.BaseProfile.Browser.Version}");

            // The Base Profile’s browser version will be updated if there is any available on our servers
            profile = await client.UpgradeProfileAsync(profile.Id);
            Console.WriteLine($"Profile's browser after update is: {profile.BaseProfile.Browser.Product} {profile.BaseProfile.Browser.Version}");

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Wait for 5 seconds
            await Task.Delay(5000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}