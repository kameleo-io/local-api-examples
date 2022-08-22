using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Microsoft.Playwright;

namespace ConnectWithPlaywrightToFirefox
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            // Search Firefox Base Profiles
            var baseProfiles = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "firefox");

            // Create a new profile with recommended settings
            // for browser fingerprint protection
            var requestBody = BuilderForCreateProfile
                .ForBaseProfile(baseProfiles[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(requestBody);

            // Start the browser
            await client.StartProfileAsync(profile.Id);

            // Connect to the browser with Playwright
            var browserWsEndpoint = $"ws://localhost:{KameleoPort}/playwright/{profile.Id}";
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Firefox.LaunchPersistentContextAsync("", new BrowserTypeLaunchPersistentContextOptions
            {
                // The Playwright framework is not designed to connect to already running 
                // browsers. To overcome this limitation, a tool bundled with Kameleo, named 
                // pw-bridge.exe will bridge the communication gap between the running Firefox 
                // instance and this playwright script.
                ExecutablePath = "<PATH_TO_KAMELEO_FOLDER>\\pw-bridge.exe",
                Args = new List<string> { $"-target {browserWsEndpoint}" },
                ViewportSize = null,
            });

            // Kameleo will open the a new page in the default browser context.
            // NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere 
            //       with Kameleo's browser fingerprint modification features.
            var page = await browser.NewPageAsync();

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
