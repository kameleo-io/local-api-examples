using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;

namespace ManageCookies
{
    class Program
    {
        private const string BaseUrl = "http://localhost:5050";
        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri(BaseUrl));
            client.SetRetryPolicy(null);

            // Search a Base Profiles
            var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "firefox");

            // Create a new profile with recommended settings
            var createProfileRequest = BuilderForCreateProfile
                .ForBaseProfile(baseProfileList[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(createProfileRequest);

            // Start the profile
            await client.StartProfileAsync(profile.Id);

            // Connect to the profile using WebDriver protocol
            var uri = new Uri($"{BaseUrl}/webdriver");
            var opts = new ChromeOptions();
            opts.AddAdditionalOption("kameleo:profileId", profile.Id.ToString());
            var webdriver = new RemoteWebDriver(uri, opts);


            // Navigate to a site which give you cookies
            webdriver.Navigate().GoToUrl("https://google.com");
            await Task.Delay(15000);

            webdriver.Navigate().GoToUrl("https://whoer.net");
            await Task.Delay(15000);

            webdriver.Navigate().GoToUrl("https://www.youtube.com");
            await Task.Delay(15000);

            webdriver.Navigate().GoToUrl("https://translate.google.com/");
            await Task.Delay(15000);

            webdriver.Navigate().GoToUrl("https://mail.google.com/");
            await Task.Delay(15000);

            // Stop the profile
            await client.StopProfileAsync(profile.Id);

            // You can list all of your cookies
            var cookieList = await client.ListCookiesAsync(profile.Id);
            Console.WriteLine("The cookies of the profile: ");
            foreach(var cookie in cookieList)
            {
                Console.WriteLine($"{cookie.Domain}, {cookie.Path}, {cookie.Name}");
            }

            // You can modify cookie or you can add new
            var newCookie = cookieList[0];
            newCookie.Value = "123";
            var cookiesArray = new List<CookieRequest> {new CookieRequest(newCookie)};
            await client.AddCookiesAsync(profile.Id, cookiesArray);

            // You can delete all cookies of the profile
            await client.DeleteCookiesAsync(profile.Id);

        }
    }
}