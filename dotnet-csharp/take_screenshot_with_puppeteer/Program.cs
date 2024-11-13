using System;
using System.IO;
using Kameleo.LocalApiClient;
using PuppeteerSharp;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
if (!int.TryParse(Environment.GetEnvironmentVariable("KAMELEO_PORT"), out var KameleoPort))
{
    KameleoPort = 5050;
}

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Chrome Base Profiles
var baseProfiles = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

// Create a new profile with recommended settings
// for browser fingerprint protection
var requestBody = BuilderForCreateProfile
    .ForBaseProfile(baseProfiles[0].Id)
    .SetName("take screenshot example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(requestBody);

// Start the Kameleo profile and connect through CDP
var browserWsEndpoint = $"ws://localhost:{KameleoPort}/puppeteer/{profile.Id}";
var browser = await Puppeteer.ConnectAsync(new ConnectOptions
{
    BrowserWSEndpoint = browserWsEndpoint,
    DefaultViewport = null
});
var page = await browser.NewPageAsync();

// Open a random page from wikipedia
await page.GoToAsync("https://en.wikipedia.org/wiki/Special:Random", WaitUntilNavigation.DOMContentLoaded);

var targetFile = Path.Combine(Environment.CurrentDirectory, Path.ChangeExtension(Path.GetRandomFileName(), ".png"));

// Take screenshot
await page.ScreenshotAsync(targetFile, new ScreenshotOptions { Type = ScreenshotType.Png });

Console.WriteLine(targetFile);

// Stop the browser by stopping the Kameleo profile
await client.StopProfileAsync(profile.Id);
