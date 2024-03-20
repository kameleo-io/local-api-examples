using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using PuppeteerSharp;

namespace ConnectWithPuppeteer
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
            var baseProfiles = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

            // Create a new profile with recommended settings
            // for browser fingerprint protection
            var requestBody = BuilderForCreateProfile
                .ForBaseProfile(baseProfiles[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(requestBody);

            // Start the Kameleo profile and connect through CDP
            var browserWsEndpoint = $"ws://localhost:{KameleoPort}/puppeteer/{profile.Id}";
            var browser = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWsEndpoint, DefaultViewport = null });
            var page = await browser.NewPageAsync();

            // Use any Puppeteer command to drive the browser
            // and enjoy full protection from bot detection products
            await page.GoToAsync("https://wikipedia.org");
            await page.ClickAsync("[name=search]");
            await page.Keyboard.TypeAsync("Chameleon");
            await page.Keyboard.PressAsync("Enter");

            // Wait for 5 seconds
            await page.WaitForTimeoutAsync(5000);

            // Stop the browser by stopping the Kameleo profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}