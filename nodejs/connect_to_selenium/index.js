const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const { Builder, By, Key, until } = require('selenium-webdriver');

(async () => {
    try {
        // Initialize the Kameleo client
        const kameleoCliUri = 'http://127.0.0.1:5050';
        const client = new KameleoLocalApiClient({
            baseUri: kameleoCliUri,
        });

        // Search one of the Base Profiles
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'chrome',
        });

        // Create a new profile with recommended settings
        // Choose one of the Base Profiles
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .build();
        const profile = await client.createProfile({
            body: createProfileRequest,
        });

        // Start the profile
        await client.startProfile(profile.id);

        // Connect to the profile using WebDriver protocol
        const builder = new Builder()
            .usingServer(`${kameleoCliUri}/webdriver`)
            .withCapabilities({
                'kameleo:profileId': profile.id,
                browserName: 'Kameleo',
            });
        const webdriver = await builder.build();

        // Use any WebDriver command to drive the browser
        // and enjoy full protection from bot detection products
        await webdriver.get('https://google.com');
        await webdriver.findElement(By.css('div[aria-modal="true"][tabindex="0"] button + button')).click();
        await webdriver.findElement(By.name('q')).sendKeys('Kameleo', Key.ENTER);
        await webdriver.wait(until.elementLocated(By.id('main')));
        const title = await webdriver.getTitle();
        console.log(`The title is ${title}`);

        // Wait for 5 seconds
        await webdriver.sleep(5000);

        // Stop the browser by stopping the Kameleo profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
