const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');

(async () => {
    try {
        const client = new KameleoLocalApiClient({
            baseUri: 'http://localhost:5050',
            noRetryPolicy: true,
        });

        // Search one of the Base Profiles
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            osFamily: 'macos'
        });

        // Create a new profile with recommended settings
        // Choose one of the Base Profiles
        // Set the launcher 'chrome' so it will start with chrome instead of chromium
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .setLauncher('chrome')
            .build();
        const profile = await client.createProfile({ body: createProfileRequest });

        // Start the profile
        await client.startProfile(profile.id);

        // Wait for 10 sec
        await new Promise((r) => setTimeout(r, 10000));

        // Stop the profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
