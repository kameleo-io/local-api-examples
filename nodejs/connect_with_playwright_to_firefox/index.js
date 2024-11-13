import { KameleoLocalApiClient, BuilderForCreateProfile } from '@kameleo/local-api-client';
import playwright from 'playwright';
import { setTimeout } from 'timers/promises';

// This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
const kameleoPort = process.env.KAMELEO_PORT || 5050;
const kameleoCliUri = `http://localhost:${kameleoPort}`;

// Initialize the Kameleo client
const client = new KameleoLocalApiClient({
    baseUri: kameleoCliUri,
    noRetryPolicy: true,
});

// Search Firefox Base Profiles
const baseProfiles = await client.searchBaseProfiles({
    deviceType: 'desktop',
    browserProduct: 'firefox',
    language: 'en-*',
});

// Create a new profile with recommended settings
// for browser fingerprint protection
const requestBody = BuilderForCreateProfile
    .forBaseProfile(baseProfiles[0].id)
    .setName('connect with Playwright to Firefox example')
    .setRecommendedDefaults()
    .build();

const profile = await client.createProfile({
    body: requestBody,
});

// Start the Kameleo profile and connect with Playwright
const browserWSEndpoint = `ws://localhost:${kameleoPort}/playwright/${profile.id}`;

// The Playwright framework is not designed to connect to already running
// browsers. To overcome this limitation, a tool bundled with Kameleo, named
// pw-bridge.exe will bridge the communication gap between the running Firefox
// instance and this playwright script.
// The exact path to the bridge executable is subject to change
let pwBridgePath = process.env.PW_BRIDGE_PATH;
if (!pwBridgePath && process.platform === 'win32') {
    pwBridgePath = `${process.env.LOCALAPPDATA}\\Programs\\Kameleo\\pw-bridge.exe`;
} else if (!pwBridgePath && process.platform === 'darwin') {
    pwBridgePath = '/Applications/Kameleo.app/Contents/MacOS/pw-bridge';
}

const browser = await playwright.firefox.launchPersistentContext('', {
    executablePath: pwBridgePath,
    args: [`-target ${browserWSEndpoint}`],
    persistent: true,
    viewport: null,
});

// Kameleo will open the a new page in the default browser context.
// NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere
//       with Kameleo's browser fingerprint modification features.
const page = await browser.newPage();

// Use any Playwright command to drive the browser
// and enjoy full protection from bot detection products
await page.goto('https://wikipedia.org');
await page.click('[name=search]');
await page.keyboard.type('Chameleon');
await page.keyboard.press('Enter');

// Wait for 5 seconds
await setTimeout(5_000);

// Stop the browser by stopping the Kameleo profile
await client.stopProfile(profile.id);
