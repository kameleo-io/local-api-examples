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

        // Search Firefox Base Profiles
        const baseProfiles = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'firefox',
            language: 'en',
        });

        // Create a new profile with recommended settings
        // for browser fingerprint protection
        const requestBody = BuilderForCreateProfile
            .forBaseProfile(baseProfiles[0].id)
            .setRecommendedDefaults()
            .build();

        const profile = await client.createProfile({ body: requestBody });
        await client.startProfile(profile.id);

        // Connect to the browser with Playwright
        const browserWSEndpoint = `ws://localhost:${kameleoPort}/playwright/${profile.id}`;
        const browser = await playwright.firefox.launch({
            // The Playwright framework is not designed to connect to already running
            // browsers. To overcome this limitation, a tool bundled with Kameleo, named
            // pw-bridge.exe will bridge the communication gap between the running Firefox
            // instance and this playwright script.
            executablePath: '<PATH_TO_KAMELEO_FOLDER>\\pw-bridge.exe',
            args: [`-target ${browserWSEndpoint}`],
        });

        // Kameleo will open the a new page in the default browser context.
        // NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere
        //       with Kameleo's browser fingerprint modification features.
        const page = await browser.newPage();

        // Use any Playwright command to drive the browser
        // and enjoy full protection from bot detection products
        await page.goto('https://google.com');
        const agreeButton = await page.$('div[aria-modal="true"][tabindex="0"] button + button');
        await agreeButton.click();
        await page.click('[name=q]');
        await page.keyboard.type('Kameleo');
        await page.keyboard.press('Enter');

        // Wait for 5 seconds
        await page.waitForTimeout(5000);

        // Stop the browser by stopping the Kameleo profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
