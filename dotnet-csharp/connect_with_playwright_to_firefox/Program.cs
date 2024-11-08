﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Microsoft.Playwright;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
if (!int.TryParse(Environment.GetEnvironmentVariable("KAMELEO_PORT"), out var KameleoPort))
{
    KameleoPort = 5050;
}

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Firefox Base Profiles
var baseProfiles = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "firefox");

// Create a new profile with recommended settings
// for browser fingerprint protection
var requestBody = BuilderForCreateProfile
    .ForBaseProfile(baseProfiles[0].Id)
    .SetName("connect with Playwright to Firefox example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(requestBody);

// Start the Kameleo profile and connect with Playwright
var browserWsEndpoint = $"ws://localhost:{KameleoPort}/playwright/{profile.Id}";

// The Playwright framework is not designed to connect to already running
// browsers. To overcome this limitation, a tool bundled with Kameleo, named
// pw-bridge.exe will bridge the communication gap between the running Firefox
// instance and this playwright script.
// The exact path to the bridge executable is subject to change.
var pwBridgePath = Environment.GetEnvironmentVariable("PW_BRIDGE_PATH");
if (pwBridgePath is null && OperatingSystem.IsWindows())
{
    var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    pwBridgePath = Path.Combine(localAppDataFolder, "Programs", "Kameleo", "pw-bridge.exe");
}
else if (pwBridgePath is null && OperatingSystem.IsMacOS())
{
    pwBridgePath = "/Applications/Kameleo.app/Contents/MacOS/pw-bridge";
}

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Firefox.LaunchPersistentContextAsync("", new BrowserTypeLaunchPersistentContextOptions
{
    ExecutablePath = pwBridgePath,
    Args = new List<string> { $"-target {browserWsEndpoint}" },
    ViewportSize = null,
});

// Kameleo will open the a new page in the default browser context.
// NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere
//       with Kameleo's browser fingerprint modification features.
var page = await browser.NewPageAsync();

// Use any Playwright command to drive the browser
// and enjoy full protection from bot detection products
await page.GotoAsync("https://wikipedia.org");
await page.ClickAsync("[name=search]");
await page.Keyboard.TypeAsync("Chameleon");
await page.Keyboard.PressAsync("Enter");

// Wait for 5 seconds
await Task.Delay(5_000);

// Stop the browser by stopping the Kameleo profile
await client.StopProfileAsync(profile.Id);
