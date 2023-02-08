const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');

(async () => {
    try {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        const kameleoPort = 5050;

        const client = new KameleoLocalApiClient({
            baseUri: `http://localhost:${kameleoPort}`,
            noRetryPolicy: true,
        });

        // Search Chrome Base Profiles
        const chromeBaseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'chrome',
        });

        // Create a new profile with recommended settings
        // Choose one of the Chrome BaseProfiles
        // You can setup here all of the profile options like WebGL, password manager and start page
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(chromeBaseProfileList[0].id)
            .setRecommendedDefaults()
            .setWebgl('noise')
            .setWebglMeta(
                'manual',
                { vendor: 'Google Inc.', renderer: 'ANGLE (Intel(R) HD Graphics 630 Direct3D11 vs_5_0 ps_5_0)' },
            )
            .setPasswordManager('enabled')
            .setStartPage('https://kameleo.io')
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
