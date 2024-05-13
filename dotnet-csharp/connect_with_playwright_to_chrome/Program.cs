using System;
using Kameleo.LocalApiClient;
using Microsoft.Playwright;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const int KameleoPort = 5050;

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Chrome Base Profiles
var baseProfiles = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

// Create a new profile with recommended settings
// for browser fingerprint protection
var requestBody = BuilderForCreateProfile
    .ForBaseProfile(baseProfiles[0].Id)
    .SetName("connect with Playwright to Chrome example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(requestBody);

// Start the Kameleo profile and connect with Playwright through CDP
var browserWsEndpoint = $"ws://localhost:{KameleoPort}/playwright/{profile.Id}";
var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.ConnectOverCDPAsync(browserWsEndpoint);

// It is recommended to work on the default context.
// NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere
//       with Kameleo's browser fingerprint modification features.
var context = browser.Contexts[0];
var page = await context.NewPageAsync();

// Use any Playwright command to drive the browser
// and enjoy full protection from bot detection products
await page.GotoAsync("https://wikipedia.org");
await page.ClickAsync("[name=search]");
await page.Keyboard.TypeAsync("Chameleon");
await page.Keyboard.PressAsync("Enter");

// Wait for 5 seconds
await page.WaitForTimeoutAsync(5000);

// Stop the browser by stopping the Kameleo profile
await client.StopProfileAsync(profile.Id);