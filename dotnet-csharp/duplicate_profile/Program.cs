using System;
using Kameleo.LocalApiClient;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const int KameleoPort = 5050;

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Chrome Base Profiles
var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

// Create a new profile with recommended settings
// Choose one of the BaseProfiles
// You can setup here all of the profile options
var createProfileRequest = BuilderForCreateProfile
    .ForBaseProfile(baseProfileList[0].Id)
    .SetName("duplicate profile example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(createProfileRequest);

var duplicatedProfile = await client.DuplicateProfileAsync(profile.Id);
Console.WriteLine($"Profile '{duplicatedProfile.Name}' is just created.");