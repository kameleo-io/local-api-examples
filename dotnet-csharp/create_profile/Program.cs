﻿using System;
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
// You can setup here all of the profile options like WebGL, password manager and start page
var createProfileRequest = BuilderForCreateProfile
    .ForBaseProfile(baseProfileList[0].Id)
    .SetName("create profile example")
    .SetRecommendedDefaults()
    .SetWebgl("noise")
    .SetWebglMeta("manual",
        new WebglMetaSpoofingOptions("Google Inc.", "ANGLE (Intel(R) HD Graphics 630 Direct3D11 vs_5_0 ps_5_0)"))
    .SetPasswordManager("enabled")
    .SetStartPage("https://kameleo.io")
    .Build();

var profile = await client.CreateProfileAsync(createProfileRequest);

// Start the profile
await client.StartProfileAsync(profile.Id);

// Wait for 10 seconds
await Task.Delay(10_000);

// Stop the profile
await client.StopProfileAsync(profile.Id);