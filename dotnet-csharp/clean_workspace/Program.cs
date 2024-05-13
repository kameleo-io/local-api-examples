using System;
using Kameleo.LocalApiClient;

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const int KameleoPort = 5050;

var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
client.SetRetryPolicy(null);

var profiles = await client.ListProfilesAsync();

foreach (var profile in profiles)
{
    await client.DeleteProfileAsync(profile.Id);
}

Console.WriteLine($"{profiles.Count} profiles deleted.");