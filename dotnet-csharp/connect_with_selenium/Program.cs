﻿using System;
using Kameleo.LocalApiClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

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
    .SetName("connect to Selenium example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(requestBody);

// Start the Kameleo profile and connect using WebDriver protocol
var uri = new Uri($"http://localhost:{KameleoPort}/webdriver");
var opts = new ChromeOptions();
opts.AddAdditionalOption("kameleo:profileId", profile.Id.ToString());
var webdriver = new RemoteWebDriver(uri, opts);
webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

// Use any WebDriver command to drive the browser
// and enjoy full protection from bot detection products
webdriver.Navigate().GoToUrl("https://wikipedia.org");
webdriver.FindElement(By.Name("search")).SendKeys("Chameleon");
webdriver.FindElement(By.Name("search")).SendKeys(Keys.Enter);
webdriver.FindElement(By.Id("content"));
var title = webdriver.Title;
Console.WriteLine($"The title is {title}");

// Stop the browser by stopping the Kameleo profile
await client.StopProfileAsync(profile.Id);
