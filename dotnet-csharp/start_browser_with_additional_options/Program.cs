using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const int KameleoPort = 5050;

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Chrome Base Profiles
var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

// Create a new profile with recommended settings
// Choose one of the Chrome BaseProfiles
var createProfileRequest = BuilderForCreateProfile
    .ForBaseProfile(baseProfileList[0].Id)
    .SetName("start browser with additional options example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(createProfileRequest);

// Provide additional settings for the webdriver when starting the browser
// Use this command to customize the browser process by adding command-line arguments
//  like '--mute-audio' or '--start-maximized'
//  or modify the native profile settings when starting the browser

// start the browser with the --mute-audio command line argument
await client.StartProfileWithOptionsAsync(profile.Id, new WebDriverSettings
{
    Arguments = new List<string> { "mute-audio" }
});
// Wait for 10 seconds
await Task.Delay(10_000);
// Stop the profile
await client.StopProfileAsync(profile.Id);

// start the browser with an additional Selenum option
await client.StartProfileWithOptionsAsync(profile.Id, new WebDriverSettings
{
    AdditionalOptions = new List<Preference> { new("pageLoadStrategy", "eager") }
});
await Task.Delay(10_000);
await client.StopProfileAsync(profile.Id);

// start the browser and also set a Chrome preference
await client.StartProfileWithOptionsAsync(profile.Id, new WebDriverSettings
{
    Preferences = new List<Preference> { new("profile.managed_default_content_settings.images", 2), }
});
await Task.Delay(10_000);
await client.StopProfileAsync(profile.Id);