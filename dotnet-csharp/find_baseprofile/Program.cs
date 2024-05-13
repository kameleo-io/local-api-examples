using System;
using Kameleo.LocalApiClient;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const int KameleoPort = 5050;

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

// Search Chrome Base Profiles
// Possible deviceType value: desktop, mobile
// Possible browserProduct value: chrome, firefox, edge
// Possible osFamily values: windows, macos, linux, android, ios
// Possible language values e.g: en-en, es-es
// You can use empty parameters as well
var baseProfileList = await client.SearchBaseProfilesAsync("desktop", "macos", "chrome", "es-es");

foreach (var profile in baseProfileList)
{
    Console.WriteLine(
        $"{profile.Os.Family} {profile.Os.Version} - {profile.Browser.Product} {profile.Browser.Version} - {profile.Language}");
}