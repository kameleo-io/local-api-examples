const { KameleoLocalApiClient } = require('@kameleo/local-api-client');

(async () => {
    try {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        const kameleoPort = 5050;

        const client = new KameleoLocalApiClient({
            baseUri: `http://localhost:${kameleoPort}`,
            noRetryPolicy: true,
        });

        const profiles = await client.listProfiles();
        for (const profile of profiles) {
            await client.deleteProfile(profile.id);
        }

        console.log(`${profiles.length} profiles deleted.`);
    } catch (error) {
        console.error(error);
    }
})();
