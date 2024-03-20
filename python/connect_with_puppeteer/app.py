from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
import pyppeteer
import time
import asyncio


async def main():
    # This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
    kameleo_port = 5050

    client = KameleoLocalApiClient(
        endpoint=f'http://localhost:{kameleo_port}',
        retry_total=0
    )

    # Search Chrome Base Profiles
    base_profiles = client.search_base_profiles(
        device_type='desktop',
        browser_product='chrome'
    )

    # Create a new profile with recommended settings
    # Choose one of the Base Profiles
    create_profile_request = BuilderForCreateProfile \
        .for_base_profile(base_profiles[0].id) \
        .set_recommended_defaults() \
        .build()
    profile = client.create_profile(body=create_profile_request)

    # Start the Kameleo profile and connect through CDP
    browser_ws_endpoint = f'ws://localhost:{kameleo_port}/puppeteer/{profile.id}'
    browser = await pyppeteer.launcher.connect(browserWSEndpoint=browser_ws_endpoint, defaultViewport=False)
    page = await browser.newPage()

    # Use any Puppeteer command to drive the browser
    # and enjoy full protection from bot detection products
    await page.goto('https://wikipedia.org')
    await page.click('[name=search')
    await page.keyboard.type('Chameleon')
    await page.keyboard.press('Enter')

    # Wait for 5 seconds
    time.sleep(5)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)

loop = asyncio.new_event_loop()
asyncio.set_event_loop(loop)
loop.run_until_complete(main())
