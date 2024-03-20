const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const playwright = require('playwright');

(async () => {
    try {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        const kameleoPort = 5050;

        const client = new KameleoLocalApiClient({
            baseUri: `http://localhost:${kameleoPort}`,
            noRetryPolicy: true,
        });

        // Search Chrome Base Profiles
        const baseProfiles = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'chrome',
            language: 'en',
        });

        // Create a new profile with recommended settings
        // for browser fingerprint protection
        const requestBody = BuilderForCreateProfile
            .forBaseProfile(baseProfiles[0].id)
            .setRecommendedDefaults()
            .build();

        const profile = await client.createProfile({
            body: requestBody,
        });

        // Start the Kameleo profile and connect with Playwright through CDP
        const browserWSEndpoint = `ws://localhost:${kameleoPort}/playwright/${profile.id}`;
        const browser = await playwright.chromium.connectOverCDP(browserWSEndpoint);

        // It is recommended to work on the default context.
        // NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere
        //       with Kameleo's browser fingerprint modification features.
        const context = browser.contexts()[0];
        const page = await context.newPage();

        // Use any Playwright command to drive the browser
        // and enjoy full protection from bot detection products
        await page.goto('https://wikipedia.org');
        await page.click('[name=search]');
        await page.keyboard.type('Chameleon');
        await page.keyboard.press('Enter');

        // Wait for 5 seconds
        await page.waitForTimeout(5000);

        // Stop the browser by stopping the Kameleo profile
        await client.stopProfile(profile.id);
    } catch (e) {
        console.log(e);
    }
})();
