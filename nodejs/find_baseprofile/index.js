const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');

(async () => {
    try {
        const client = new KameleoLocalApiClient({
            baseUri: 'http://localhost:5050',
            noRetryPolicy: true,
        });

        // Search Chrome Base Profiles
        // Possible deviceType value: desktop, mobile
        // Possible browserProduct value: chrome, firefox, edge
        // Possible osFamily values: windows, macos, linux, android, ios
        // Possible language values e.g: en-en, es,es
        // You can use empty parameters as well
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'chrome',
            osFamily: 'macos',
            language: 'es-es',
        });

        console.log(baseProfileList);

        // Create a new profile with recommended settings
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .build();
        const profile = await client.createProfile({ body: createProfileRequest });

        // Start the profile
        await client.startProfile(profile.id);

        // Wait for 10 seconds
        await new Promise((r) => setTimeout(r, 10000));

        // Stop the profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
