import { KameleoLocalApiClient, BuilderForCreateProfile } from '@kameleo/local-api-client';
import { setTimeout } from 'timers/promises';
import randomInteger from 'random-int';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

// Search Chrome Base Profiles
// Possible deviceType value: desktop, mobile
// Possible browserProduct value: chrome, firefox, edge
// Possible osFamily values: windows, macos, linux, android, ios
// Possible language values e.g: en-en, es-es
// You can use empty parameters as well
const chromeBaseProfileList = await client.searchBaseProfiles({
    deviceType: 'desktop',
    osFamily: 'windows',
    browserProduct: 'chrome',
    language: 'es-es',
});

// Create a new profile with recommended settings
// Choose one of the Chrome BaseProfiles
// You can setup here all of the profile options like WebGL, password manager and start page
const createProfileRequest = BuilderForCreateProfile
    .forBaseProfile(chromeBaseProfileList[randomInteger(chromeBaseProfileList.length)].id)
    .setName('create profile example')
    .setRecommendedDefaults()
    .setWebgl('noise')
    .setWebglMeta(
        'manual',
        {
            vendor: 'Google Inc.', renderer: 'ANGLE (Intel(R) HD Graphics 630 Direct3D11 vs_5_0 ps_5_0)',
        },
    )
    .setPasswordManager('enabled')
    .setStartPage('https://kameleo.io')
    .build();
const profile = await client.createProfile({
    body: createProfileRequest,
});

// Start the profile
await client.startProfile(profile.id);

// Wait for 10 seconds
await setTimeout(10_000);

// Stop the profile
await client.stopProfile(profile.id);

let duplicatedProfile = await client.duplicateProfile(profile.id);
console.log(`Profile '${duplicatedProfile.name}' is just created`);

// Change every property that you want to update on the duplicated profile
duplicatedProfile.name = 'duplicate profile example';

// Send the update request and the response will be your updated profile
duplicatedProfile = await client.updateProfile(duplicatedProfile.id, {
    body: duplicatedProfile,
});

// Start the profile
await client.startProfile(duplicatedProfile.id);

// Wait for 10 seconds
await setTimeout(10_000);

// Stop the profile
await client.stopProfile(duplicatedProfile.id);

// Delete original profile
// Profiles need to be deleted explicitly becase they are persisted so they are available after restarting Kameleo
await client.deleteProfile(profile.id);
