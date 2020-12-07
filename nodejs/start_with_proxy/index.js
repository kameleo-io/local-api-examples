const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');

(async () => {
    try {
        const client = new KameleoLocalApiClient({
            baseUri: 'http://localhost:5050',
            noRetryPolicy: true,
        });

        // Search Chrome Base Profiles
        const chromeBaseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            language: 'es-es'
        });

        // Create a new profile with recommended settings
        // Choose one of the Chrome BaseProfiles
        // You can set your proxy up in the setProxy method
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(chromeBaseProfileList[0].id)
            .setRecommendedDefaults()
            .setProxy(
                'socks5',
                {
                    host: '<proxy_host>',
                    port: 1080,
                    id: '<username>',
                    secret: '<password>',
                },
            )
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
