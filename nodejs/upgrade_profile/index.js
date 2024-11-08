import { KameleoLocalApiClient, BuilderForCreateProfile } from '@kameleo/local-api-client';
import { setTimeout } from 'timers/promises';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

// Search for a Desktop Base Profile with Windows OS and Chrome browser
const baseProfileList = await client.searchBaseProfiles({
    deviceType: 'desktop',
    osFamily: 'windows',
    browserProduct: 'chrome',
});

// Find a Base Profile with the oldest available version of chrome
const baseProfile = baseProfileList.sort((a, b) => ((a.browser.major > b.browser.major) ? 1 : -1))[0];

// Create a new profile with recommended settings
// Choose one of the BaseProfiles
const createProfileRequest = BuilderForCreateProfile
    .forBaseProfile(baseProfile.id)
    .setName('upgrade profiles example')
    .setRecommendedDefaults()
    .build();
let profile = await client.createProfile({
    body: createProfileRequest,
});

console.log(`Profile's browser before update is: ${profile.baseProfile.browser.product} ${profile.baseProfile.browser.version}`);

// The Base Profile’s browser version will be updated if there is any available on our servers
profile = await client.upgradeProfile(profile.id, {
    body: profile,
});
console.log(`Profile's browser after update is: ${profile.baseProfile.browser.product} ${profile.baseProfile.browser.version}`);

// Start the profile
await client.startProfile(profile.id);

// Wait for 5 seconds
await setTimeout(5_000);

// Stop the profile
await client.stopProfile(profile.id);
