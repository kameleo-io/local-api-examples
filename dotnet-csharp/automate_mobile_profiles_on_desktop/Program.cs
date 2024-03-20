using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace AutomateMobileProfilesOnDesktop
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));

            // Search for a mobile Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync("mobile", "ios", "safari", "en-us");

            // Create a new profile with recommended settings
            // Choose one of the Base Profiles
            // Set the launcher to 'chromium' so the mobile profile will be started in Chroma browser
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .SetLauncher("chromium")
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Start the profile
            await client.StartProfileWithOptionsAsync(profile.Id, new WebDriverSettings()
            {
                AdditionalOptions = new List<Preference>
                {
                    // This allows you to click on elements using the cursor when emulating a touch screen in the browser.
                    // If you leave this out, your script may time out after clicks and fail.
                    new Preference("disableTouchEmulation", true),
                }
            });

            // Connect to the running browser instance using WebDriver
            var uri = new Uri($"http://localhost:{KameleoPort}/webdriver");
            var opts = new ChromeOptions();
            opts.AddAdditionalOption("kameleo:profileId", profile.Id.ToString());
            var webdriver = new RemoteWebDriver(uri, opts);

            // Use any WebDriver command to drive the browser
            // and enjoy full protection from bot detection products
            webdriver.Navigate().GoToUrl("https://wikipedia.org");
            webdriver.FindElement(By.Name("search")).SendKeys("Chameleon");
            webdriver.FindElement(By.Name("search")).SendKeys(Keys.Enter);
            webdriver.FindElement(By.Id("content"));
            var title = webdriver.Title;
            Console.WriteLine($"The title is {title}");

            await Task.Delay(TimeSpan.FromSeconds(5));

            // Stop the browser by stopping the Kameleo profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}