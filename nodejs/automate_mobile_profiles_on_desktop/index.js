const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const {
    Builder, By, Key, until
} = require('selenium-webdriver');

(async () => {
    try {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        const kameleoPort = 5050;

        const client = new KameleoLocalApiClient({
            baseUri: 'http://localhost:5050',
            noRetryPolicy: true,
        });

        // Search for a mobile Base Profiles
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'mobile',
            osFamily: 'ios',
            browserProduct: 'safari',
            language: 'en-us',
        });

        // Create a new profile with recommended settings
        // Choose one of the Base Profiles
        // Set the launcher to 'chromium' so the mobile profile will be started in Chromium by Kameleo
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .setLauncher('chromium')
            .build();
        const profile = await client.createProfile({ body: createProfileRequest });

        // Start the profile
        await client.startProfileWithWebDriverSettings(profile.id, {
            body: {
                // This allows you to click on elements using the cursor when emulating a touch screen in the brower.
                // If you leave this out, your script may time out after clicks and fail.
                additionalOptions: [
                    {
                        key: 'disableTouchEmulation',
                        value: true,
                    },
                ],
            },
        });

        // In this example we show how you can automate the mobile profile with Selenium
        // You can also do this with Puppeteer or Playwright
        const builder = new Builder()
            .usingServer(`http://localhost:${kameleoPort}/webdriver`)
            .withCapabilities({
                'kameleo:profileId': profile.id,
                browserName: 'Kameleo',
            });
        const webdriver = await builder.build();

        // Use any WebDriver command to drive the browser
        // and enjoy full protection from bot detection products
        await webdriver.get('https://google.com');
        const button = await webdriver.findElement(By.css('div[aria-modal="true"][tabindex="0"] button + button'));
        webdriver.executeScript('arguments[0].scrollIntoView();', button);
        await webdriver.sleep(1000);
        await button.click();
        await webdriver.findElement(By.name('q')).sendKeys('Kameleo', Key.ENTER);
        await webdriver.wait(until.elementLocated(By.id('main')));
        const title = await webdriver.getTitle();
        console.log(`The title is ${title}`);

        // Wait for 5 seconds
        await webdriver.sleep(5000);

        // Stop the browser by stopping the Kameleo profile
        await client.stopProfile(profile.id);

        webdriver.Quit();

        // Stop the browser by stopping the Kameleo profile
        await client.stopProfile(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
