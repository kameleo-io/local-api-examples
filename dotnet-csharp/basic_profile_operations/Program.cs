using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
if (!int.TryParse(Environment.GetEnvironmentVariable("KAMELEO_PORT"), out var KameleoPort))
{
    KameleoPort = 5050;
}

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Chrome Base Profiles
// Possible deviceType value: desktop, mobile
// Possible browserProduct value: chrome, firefox, edge
// Possible osFamily values: windows, macos, linux, android, ios
// Possible language values e.g: en-en, es-es
// You can use empty parameters as well
var baseProfileList = await client.SearchBaseProfilesAsync("desktop", "windows", "chrome", "es-es");

Console.WriteLine("Available base profiles:");
foreach (var baseProfile in baseProfileList)
{
    Console.WriteLine(
        $"{baseProfile.Os.Family} {baseProfile.Os.Version} - {baseProfile.Browser.Product} {baseProfile.Browser.Version} - {baseProfile.Language}");
}

// Select a random base profile
var selectedBaseProfile = baseProfileList[Random.Shared.Next(0, baseProfileList.Count - 1)];

Console.WriteLine(
    $"Selected base profile: {selectedBaseProfile.Os.Family} {selectedBaseProfile.Os.Version} - " +
    $"{selectedBaseProfile.Browser.Product} {selectedBaseProfile.Browser.Version} - {selectedBaseProfile.Language}");

// Create a new profile with recommended settings using the selected Chrome BaseProfiles
// You can setup here all of the profile options like WebGL, password manager and start page
var createProfileRequest = BuilderForCreateProfile
    .ForBaseProfile(selectedBaseProfile.Id)
    .SetName("create profile example")
    .SetRecommendedDefaults()
    .SetWebgl("noise")
    .SetWebglMeta("manual",
        new WebglMetaSpoofingOptions("Google Inc.", "ANGLE (Intel(R) HD Graphics 630 Direct3D11 vs_5_0 ps_5_0)"))
    .SetPasswordManager("enabled")
    .SetStartPage("https://kameleo.io")
    .Build();

var profile = await client.CreateProfileAsync(createProfileRequest);

Console.WriteLine($"Id of the created profile: {profile.Id}");

// Start the profile
await client.StartProfileAsync(profile.Id);

// Wait for 10 seconds
await Task.Delay(10_000);

// Stop the profile
await client.StopProfileAsync(profile.Id);

// Duplicate the previously created profile
var duplicatedProfile = await client.DuplicateProfileAsync(profile.Id);

Console.WriteLine($"Profile '{duplicatedProfile.Name}' is created");

// Change every property that you want to update on the duplicate profile
// Others should be the same
var updateRequestBody = new UpdateProfileRequest(duplicatedProfile)
{
    Name = "duplicate profile example",
    WebglMeta = new WebglMetaSpoofingTypeWebglMetaSpoofingOptionsMultiLevelChoice("automatic")
};

// Send the update request and the response will be your updated profile
duplicatedProfile = await client.UpdateProfileAsync(duplicatedProfile.Id, updateRequestBody);

// Start the profile
await client.StartProfileAsync(duplicatedProfile.Id);

// Wait for 10 seconds
await Task.Delay(10_000);

// Stop the profile
await client.StopProfileAsync(duplicatedProfile.Id);

// Delete original profile
// Profiles need to be deleted explicitly becase they are persisted so they are available after restarting Kameleo
await client.DeleteProfileAsync(profile.Id);
