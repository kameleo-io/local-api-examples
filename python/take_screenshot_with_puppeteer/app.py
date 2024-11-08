from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
import asyncio
import pyppeteer
import random
import string
import os


async def main():
    # This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
    kameleo_port = os.getenv('KAMELEO_PORT', '5050')

    client = KameleoLocalApiClient(
        endpoint=f'http://localhost:{kameleo_port}',
        retry_total=0
    )

    # Search Chrome Base Profiles
    base_profiles = client.search_base_profiles(
        device_type='desktop',
        browser_product='chrome'
    )

    # Create a new profile with recommended settings for browser fingerprinting protection
    # Choose one of the Chrome BaseProfiles
    # You can setup here all of the profile options like WebGL
    create_profile_request = BuilderForCreateProfile \
        .for_base_profile(base_profiles[0].id) \
        .set_name('take screenshot example') \
        .set_recommended_defaults() \
        .build()
    profile = client.create_profile(body=create_profile_request)

    # Start the Kameleo profile and connect through CDP
    browser_ws_endpoint = f'ws://localhost:{kameleo_port}/puppeteer/{profile.id}'
    browser = await pyppeteer.launcher.connect(browserWSEndpoint=browser_ws_endpoint, defaultViewport=False)
    page = await browser.newPage()

    # Open a random page from wikipedia
    await page.goto('https://en.wikipedia.org/wiki/Special:Random')
    
    res = ''.join(random.choices(string.ascii_lowercase + string.digits, k=10))
    
    # Take screenshot
    await page.screenshot({'path': f'{res}.png', 'fullPage': True})

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)

loop = asyncio.new_event_loop()
asyncio.set_event_loop(loop)
loop.run_until_complete(main())
