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

// Search Chrome Base Profiles
const chromeBaseProfileList = await client.searchBaseProfiles({
    deviceType: 'desktop',
    browserProduct: 'chrome',
});

// Create a new profile with recommended settings
// Choose one of the Chrome BaseProfiles
const createProfileRequest = BuilderForCreateProfile
    .forBaseProfile(chromeBaseProfileList[0].id)
    .setName('start browser with additional options example')
    .setRecommendedDefaults()
    .build();
const profile = await client.createProfile({
    body: createProfileRequest,
});

// Provide additional settings for the webdriver when starting the browser
// Use this command to customize the browser process by adding command-line arguments
//  like '--mute-audio' or '--start-maximized'
//  or modify the native profile settings when starting the browser

// start the browser with the --mute-audio command line argument
await client.startProfileWithOptions(profile.id, {
    body: {
        arguments: [
            'mute-audio',
        ],
    },
});
// Wait for 10 seconds
await setTimeout(10_000);
// Stop the profile
await client.stopProfile(profile.id);

// start the browser with an additional Selenum option
await client.startProfileWithOptions(profile.id, {
    body: {
        additionalOptions: [
            {
                key: 'pageLoadStrategy',
                value: 'eager',
            },
        ],
    },
});
await setTimeout(10_000);
await client.stopProfile(profile.id);

// start the browser and also set a Chrome preference
await client.startProfileWithOptions(profile.id, {
    body: {
        preferences: [
            {
                key: 'profile.managed_default_content_settings.images',
                value: 2,
            },
        ],
    },
});
await setTimeout(10_000);
await client.stopProfile(profile.id);
