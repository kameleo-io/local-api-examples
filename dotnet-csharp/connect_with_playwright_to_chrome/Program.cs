using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Microsoft.Playwright;

namespace ConnectWithPlaywrightToChrome
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

            // Start the browser
            await client.StartProfileAsync(profile.Id);

            // Connect to the browser with Playwright through CDP
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
            await page.GotoAsync("https://google.com");
            await page.ClickAsync("div[aria-modal='true'][tabindex='0'] button + button");
            await page.ClickAsync("[name=q]");
            await page.Keyboard.TypeAsync("Kameleo");
            await page.Keyboard.PressAsync("Enter");

            // Wait for 5 seconds
            await page.WaitForTimeoutAsync(5000);

            // Stop the browser by stopping the Kameleo profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}
