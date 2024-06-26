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
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(chromeBaseProfileList[0].id)
            .setName('start browser with additional options example')
            .setRecommendedDefaults()
            .build();
        const profile = await client.createProfile({
            body: createProfileRequest,
        });

        // Provide additional settings for the webdriver when starting the browser
        // Use this command to customize the browser process by adding command-line arguments
        //  like '--mute-audio' or '--start-maximized'
        //  or modify the native profile settings when starting the browser

        // start the browser with the --mute-audio command line argument
        await client.startProfileWithOptions(profile.id, {
            body: {
                argumentsProperty: [
                    'mute-audio',
                ],
            },
        });
        // Wait for 10 seconds
        await new Promise((r) => setTimeout(r, 10_000));
        // Stop the profile
        await client.stopProfile(profile.id);

        // start the browser with an additional Selenum option
        await client.startProfileWithOptions(profile.id, {
            body: {
                additionalOptions: [
                    {
                        key: 'pageLoadStrategy',
                        value: 'eager',
                    },
                ],
            },
        });
        await new Promise((r) => setTimeout(r, 10_000));
        await client.stopProfile(profile.id);

        // start the browser and also set a Chrome preference
        await client.startProfileWithOptions(profile.id, {
            body: {
                preferences: [
                    {
                        key: 'profile.managed_default_content_settings.images',
                        value: 2,
                    },
                ],
            },
        });
        await new Promise((r) => setTimeout(r, 10_000));
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
