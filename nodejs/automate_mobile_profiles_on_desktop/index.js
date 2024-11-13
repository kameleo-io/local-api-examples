import { KameleoLocalApiClient, BuilderForCreateProfile } from '@kameleo/local-api-client';
import { Builder, By, Key, until } from 'selenium-webdriver';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

// Search for mobile Base Profiles that match specific criteria
// In this example, we are looking for iOS mobile profiles with Safari browser and language set to English (US)
const baseProfileList = await client.searchBaseProfiles({
    deviceType: 'mobile',
    osFamily: 'ios',
    browserProduct: 'safari',
    language: 'en-us',
});

// Create a new profile using one of the Base Profiles obtained above.
// Set it with recommended default settings and use Chromium as the launcher.
const createProfileRequest = BuilderForCreateProfile
    .forBaseProfile(baseProfileList[0].id)
    .setName('automate mobile profiles on desktop example')
    .setRecommendedDefaults()
    .setLauncher('chromium')
    .build();
const profile = await client.createProfile({
    body: createProfileRequest,
});

// Start the newly created profile
// We disable touch emulation so that we can click on elements when emulating a mobile profile
await client.startProfileWithOptions(profile.id, {
    body: {
        additionalOptions: [
            {
                key: 'disableTouchEmulation',
                value: true,
            },
        ],
    },
});

// Build the WebDriver for automating the mobile profile
// Configure it to use Kameleo WebDriver server
const builder = new Builder()
    .usingServer(`${kameleoCliUri}/webdriver`)
    .withCapabilities({
        'kameleo:profileId': profile.id,
        browserName: 'Kameleo',
    });
const webdriver = await builder.build();

// Automate the browser to perform a Wikipedia search and print the page title
await webdriver.get('https://wikipedia.org');
await webdriver.findElement(By.name('search')).sendKeys('Chameleon', Key.ENTER);
await webdriver.wait(until.elementLocated(By.id('content')));
const title = await webdriver.getTitle();
console.log(`The title is ${title}`);

// Wait for 5 seconds
await webdriver.sleep(5000);

// Stop the browser by stopping the Kameleo profile
await client.stopProfile(profile.id);
