const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const { Builder, By, Key, until } = require('selenium-webdriver');

(async () => {
    try {
        // Initialize the Kameleo client
        const kameleoCliUri = 'http://127.0.0.1:5050';
        const client = new KameleoLocalApiClient({
            baseUri: kameleoCliUri,
        });

        // Search for mobile Base Profiles that match specific criteria
        // In this example, we are looking for iOS mobile profiles with Safari browser and language set to English (US)
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'mobile',
            osFamily: 'ios',
            browserProduct: 'safari',
            language: 'en-us',
        });

        // Create a new profile using one of the Base Profiles obtained above.
        // Set it with recommended default settings and use Chromium as the launcher.
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .setLauncher('chromium')
            .build();
        const profile = await client.createProfile({
            body: createProfileRequest,
        });

        // Start the newly created profile
        // We disable touch emulation so that we can click on elements when emulating a mobile profile
        await client.startProfileWithOptions(profile.id, {
            body: {
                additionalOptions: [
                    {
                        key: 'disableTouchEmulation',
                        value: true,
                    },
                ],
            },
        });

        // Build the WebDriver for automating the mobile profile
        // Configure it to use Kameleo WebDriver server
        const builder = new Builder()
            .usingServer(`${kameleoCliUri}/webdriver`)
            .withCapabilities({
                'kameleo:profileId': profile.id,
                browserName: 'Kameleo',
            });
        const webdriver = await builder.build();

        // Automate the browser to perform a Google search and print the page title
        await webdriver.get('https://google.com');
        const button = await webdriver.findElement(By.css('div[aria-modal="true"][tabindex="0"] button + button'));
        webdriver.executeScript('arguments[0].scrollIntoView();', button);
        await webdriver.sleep(1000); // Wait for 1 second to simulate human-like interaction
        await button.click();
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
