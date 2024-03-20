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
            language: 'es-es',
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
                    host: process.env.PROXY_HOST || '<your_proxy_host>',
                    port: Number(process.env.PROXY_PORT) || 1080,
                    id: process.env.PROXY_USERNAME || '<your_username>',
                    secret: process.env.PROXY_PASSWORD || '<your_password>',
                },
            )
            .build();
        const profile = await client.createProfile({
            body: createProfileRequest,
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
