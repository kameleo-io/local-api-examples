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
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'chrome'
        });


        // Create a new profile with recommended settings
        // Choose one of the BaseProfiles
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .build();
        let profile = await client.createProfile({ body: createProfileRequest });


        // Start the profile
        await client.startProfile(profile.id);

        // Wait for 10 seconds
        await new Promise((r) => setTimeout(r, 5000));

        // Stop the profile
        await client.stopProfile(profile.id);

        // The duplicated profile is in the memory only and will be deleted when the Kameleo.CLI is closed unless you save it.
        var duplicatedProfile = await client.duplicateProfile(profile.id);
        console.log(`Profile '${duplicatedProfile.name}' is just created`);
    } catch (error) {
        console.error(error);
    }
})();
