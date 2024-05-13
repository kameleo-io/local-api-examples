using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const int KameleoPort = 5050;

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search a Base Profiles
var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop");

// Create a new profile with recommended settings
// Choose one of the BaseProfiles
// You can setup here all of the profile options
var createProfileRequest = BuilderForCreateProfile
    .ForBaseProfile(baseProfileList[0].Id)
    .SetName("update profile example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(createProfileRequest);

// Change every property that you want to update
// Others should be the same
var updateRequestBody = new UpdateProfileRequest(profile)
{
    StartPage = "https://www.google.com",
    Canvas = "noise",
};

// Send the update request and the response will be your new profile
profile = await client.UpdateProfileAsync(profile.Id, updateRequestBody);

// Start the profile
await client.StartProfileAsync(profile.Id);

// Wait for 10 seconds
await Task.Delay(10_000);

// Stop the profile
await client.StopProfileAsync(profile.Id);