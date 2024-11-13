using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;
using Kameleo.LocalApiClient.Models;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
if (!int.TryParse(Environment.GetEnvironmentVariable("KAMELEO_PORT"), out var KameleoPort))
{
    KameleoPort = 5050;
}

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search a Base Profiles
var baseProfileList = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "firefox");

// Create a new profile with recommended settings
var createProfileRequest = BuilderForCreateProfile
    .ForBaseProfile(baseProfileList[0].Id)
    .SetName("manage cookies example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(createProfileRequest);

// Start the Kameleo profile and connect to it using WebDriver protocol
var uri = new Uri($"http://localhost:{KameleoPort}/webdriver");
var opts = new FirefoxOptions();
opts.AddAdditionalOption("kameleo:profileId", profile.Id.ToString());
var webdriver = new RemoteWebDriver(uri, opts);


// Navigate to a site which give you cookies
webdriver.Navigate().GoToUrl("https://www.nytimes.com");
await Task.Delay(5_000);

webdriver.Navigate().GoToUrl("https://whoer.net");
await Task.Delay(5_000);

webdriver.Navigate().GoToUrl("https://www.youtube.com");
await Task.Delay(5_000);

// Stop the profile
await client.StopProfileAsync(profile.Id);

// You can list all of your cookies
var cookieList = await client.ListCookiesAsync(profile.Id);
Console.WriteLine("The cookies of the profile: ");
foreach (var cookie in cookieList)
{
    Console.WriteLine($"{cookie.Domain}, {cookie.Path}, {cookie.Name}");
}

// You can modify cookie or you can add new
var newCookie = cookieList[0];
newCookie.Value = "123";
var cookiesArray = new List<CookieRequest> { new CookieRequest(newCookie) };
await client.AddCookiesAsync(profile.Id, cookiesArray);

// You can delete all cookies of the profile
await client.DeleteCookiesAsync(profile.Id);
