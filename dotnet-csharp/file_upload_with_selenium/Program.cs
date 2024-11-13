using Kameleo.LocalApiClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    .SetName("file upload example")
    .SetRecommendedDefaults()
    .Build();

var profile = await client.CreateProfileAsync(requestBody);

// Start the Kameleo profile and connect using WebDriver protocol
var uri = new Uri($"http://localhost:{KameleoPort}/webdriver");
var opts = new ChromeOptions();
opts.AddAdditionalOption("kameleo:profileId", profile.Id.ToString());
var webdriver = new RemoteWebDriver(uri, opts);

// Open file.io
webdriver.Url = "https://www.file.io/";
await Task.Delay(3000);

// Accept cookies, optional step
try
{
    var cookieConsentButton = webdriver.FindElementByClassName("fc-cta-consent");
    cookieConsentButton.Click();
}
catch
{
}

// Upload file
var filePath = Path.Combine(Environment.CurrentDirectory, "kameleo.png");
var fileInput = webdriver.FindElement(By.CssSelector("input[type=file]"));
fileInput.SendKeys(filePath);

// Wait for upload to complete
var wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(15));
wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
wait.Until(d => d.FindElements(By.TagName("div")).Any(i => i.Text == "Your file is ready to share!"));

// Find uploaded file url
var fileUrlDiv = webdriver.FindElementsByTagName("div").FirstOrDefault(i => i.Displayed && i.GetAttribute("tabindex") == "0");
var fileUrl = fileUrlDiv.Text;

Console.WriteLine(fileUrl);

// View uploaded file
webdriver.Navigate().GoToUrl(fileUrl);

await Task.Delay(5_000);

// Stop the browser by stopping the Kameleo profile
await client.StopProfileAsync(profile.Id);
