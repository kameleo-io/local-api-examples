import { KameleoLocalApiClient, BuilderForCreateProfile } from '@kameleo/local-api-client';
import { Builder } from 'selenium-webdriver';
import randomInteger from 'random-int';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

// Search one of the Base Profiles
const baseProfileList = await client.searchBaseProfiles({
    deviceType: 'desktop',
    browserProduct: 'firefox',
});

// Create a new profile with recommended settings
// Choose one of the Base Profiles
const createProfileRequest = BuilderForCreateProfile
    .forBaseProfile(baseProfileList[0].id)
    .setName('cookie robot example')
    .setRecommendedDefaults()
    .build();
const profile = await client.createProfile({
    body: createProfileRequest,
});

// Start the Kameleo profile and connect to the profile using WebDriver protocol
const builder = new Builder()
    .usingServer(`http://localhost:${kameleoPort}/webdriver`)
    .withCapabilities({
        'kameleo:profileId': profile.id,
        browserName: 'Kameleo',
    });
const webdriver = await builder.build();

const allSites = [
    'instagram.com',
    'linkedin.com',
    'ebay.com',
    'pinterest.com',
    'reddit.com',
    'cnn.com',
    'bbc.co.uk',
    'nytimes.com',
    'reuters.com',
    'theguardian.com',
    'foxnews.com',
];

// Select sites to collect cookies from
const sitesToVisit = [];
while (sitesToVisit.length < 5) {
    const site = allSites[randomInteger(allSites.length - 1)];
    if (!sitesToVisit.includes(site)) {
        sitesToVisit.push(site);
    }
}

// Warm up profile by visiting the randomly selected sites
for (let i = 0; i < sitesToVisit.length; i++) {
    await webdriver.get(`https://${sitesToVisit[i]}`);
    await webdriver.sleep(randomInteger(5000, 15000));
}

// Stop the profile
await client.stopProfile(profile.id);
