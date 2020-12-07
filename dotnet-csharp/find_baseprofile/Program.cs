using System;
using System.Threading.Tasks;
using Kameleo.LocalApiClient;

namespace FindBaseprofile
{
    class Program
    {
        static async Task Main()
        {
            var client = new KameleoLocalApiClient();
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
                Console.WriteLine(profile.Browser.Product);
            }
        }
    }
}

