const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const path = require('path');

(async () => {
    try {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        const kameleoPort = 5050;

        const client = new KameleoLocalApiClient({
            baseUri: `http://localhost:${kameleoPort}`,
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
        const exportPath = path.join(__dirname, 'test.kameleo');

        await client.exportProfile(profile.id, {
            body: {
                path: exportPath,
            },
        });
        console.log('Profile has been exported to', __dirname);

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
        await new Promise((r) => setTimeout(r, 10_000));

        // Stop the profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
