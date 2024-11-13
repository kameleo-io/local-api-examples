import { KameleoLocalApiClient, BuilderForCreateProfile } from '@kameleo/local-api-client';
import { cwd } from 'process';
import path from 'path';
import { setTimeout } from 'timers/promises';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

// Search one of the Base Profiles
const baseProfileList = await client.searchBaseProfiles({
    deviceType: 'desktop',
});

// Create a new profile with recommended settings
const createProfileRequest = BuilderForCreateProfile
    .forBaseProfile(baseProfileList[0].id)
    .setName('profile export import example')
    .setRecommendedDefaults()
    .build();
let profile = await client.createProfile({
    body: createProfileRequest,
});

// export the profile to a given path
const exportPath = path.join(cwd(), 'test.kameleo');

await client.exportProfile(profile.id, {
    body: {
        path: exportPath,
    },
});
console.log('Profile has been exported to', cwd());

// You have to delete this profile if you want to import back
await client.deleteProfile(profile.id);

// import the profile from the given url
profile = await client.importProfile({
    body: {
        path: exportPath,
    },
});

// Start the profile
await client.startProfile(profile.id);

// Wait for 10 seconds
await setTimeout(10_000);

// Stop the profile
await client.stopProfile(profile.id);
