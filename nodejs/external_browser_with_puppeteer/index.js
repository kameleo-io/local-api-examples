const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const puppeteer = require('puppeteer');

async function runPuppeteer(port) {
    const browser = await puppeteer.launch({
        headless: false,
        defaultViewport: null,
        args: [ `--proxy-server=http://127.0.0.1:${port}` ]
    });

    const page = await browser.newPage();
    const url = 'https://help.kameleo.io/hc/en-us';

    try {
        await page.goto(url)
        await new Promise((r) => setTimeout(r, 10000));
        await browser.close();
    } catch (e) {
        console.log(e);
    }
}

(async () => {
    try {
        const client = new KameleoLocalApiClient({
            baseUri: 'http://localhost:5050',
            noRetryPolicy: true,
        });

        // Search Chrome Base Profiles
        const chromeBaseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'chrome',
        });

        // Create a new profile with recommended settings
        // Choose one of the Chrome BaseProfiles
        // You can setup here all of the profile options like Webgl
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(chromeBaseProfileList[0].id)
            .setRecommendedDefaults()
            .setLauncher('external')
            .build();

        const profile = await client.createProfile({ body: createProfileRequest });

        // Start the profile
        const port = await client.startProfile(profile.id);

        // Here you can use the external browser
        await runPuppeteer(port.externalSpoofingEnginePort);

        // Wait for 10 seconds
        await new Promise((r) => setTimeout(r, 10000));

        // Stop the profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();

