using PuppeteerSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;

namespace external_browser_with_puppeteer
{
    class Program
    {
        static async Task Main()
        {
            var client = new KameleoLocalApiClient();
            client.SetRetryPolicy(null);

            // Search Chrome Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync("desktop", null, "chrome", null);

            // Create a new profile with recommended settings
            // Choose one of the Chrome BaseProfiles
            // You can setup here all of the profile options like Webgl
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetLauncher("external")
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Start the profile
            var port = await client.StartProfileAsync(profile.Id);

            if (port.ExternalSpoofingEnginePort != null)
            {
                await RunPuppeteer(port.ExternalSpoofingEnginePort.Value);
            }

            // Wait for 10 seconds
            await Task.Delay(10000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);
        }

        private static async Task RunPuppeteer(int externalSpoofingEnginePort)
        {
            var _ = new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).Result;

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                Args = new[]
                    {
                        ("--proxy-server=http://127.0.0.1:" + externalSpoofingEnginePort)
                    }
            });

            try
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync("https://help.kameleo.io");
                Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (browser != null)
                {
                    await browser.CloseAsync();
                }
            }

        }
    }
}