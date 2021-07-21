const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');
const { Builder, By, Key } = require('selenium-webdriver');

(async () => {
    const kameleoBaseUrl = 'http://localhost:5050';

    try {
        const client = new KameleoLocalApiClient({
            baseUri: kameleoBaseUrl,
            noRetryPolicy: true,
        });

        // Search one of the Base Profiles
        const baseProfileList = await client.searchBaseProfiles({
            deviceType: 'desktop',
            browserProduct: 'firefox'
        });

        // Create a new profile with recommended settings
        // Choose one of the Base Profiles
        const createProfileRequest = BuilderForCreateProfile
            .forBaseProfile(baseProfileList[0].id)
            .setRecommendedDefaults()
            .build();
        const profile = await client.createProfile({ body: createProfileRequest });

        // Start the profile
        await client.startProfile(profile.id);

        // Connect to the profile using WebDriver protocol
        const builder = new Builder()
            .usingServer(`${kameleoBaseUrl}/webdriver`)
            .withCapabilities({
                'kameleo:profileId': profile.id,
                browserName: 'Kameleo',
            });
        const webdriver = await builder.build();

        // Navigate to a site which give you cookies
        await webdriver.get('https://google.com');
        await webdriver.sleep(15000);

        await webdriver.get('https://whoer.net');
        await webdriver.sleep(15000);

        await webdriver.get('https://www.youtube.com');
        await webdriver.sleep(15000);

        await webdriver.get('https://translate.google.com/');
        await webdriver.sleep(15000);

        // Stop the profile
        await client.stopProfile(profile.id);

        // You can list all of your cookies
        const cookieList = await client.listCookies(profile.id);
        console.log('The cookies of the profile: ', cookieList);

        // You can modify cookie or you can add new
        const newCookie = cookieList[0];
        newCookie.value = '123';
        const cookiesArray = new Array(newCookie);
        await client.addCookies(profile.id, { body: cookiesArray });

        // You can delete all cookies of the profile
        await client.deleteCookies(profile.id);
    } catch (error) {
        console.error(error);
    }
})();
