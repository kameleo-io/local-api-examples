import { KameleoLocalApiClient } from '@kameleo/local-api-client';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

const profiles = await client.listProfiles();
await Promise.all(profiles.map(profile => client.deleteProfile(profile.id)));
console.log(`${profiles.length} profiles deleted.`);
