from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models.problem_response_py3 import ProblemResponseException
from playwright.sync_api import sync_playwright
import time
import json


try:
    # This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
    kameleo_port = 5050

    client = KameleoLocalApiClient(f'http://localhost:{kameleo_port}')

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

    # Start the browser profile
    client.start_profile(profile.id)

    # Connect to the browser with Playwright through CDP
    browser_ws_endpoint = f'ws://localhost:{kameleo_port}/playwright/{profile.id}'
    with sync_playwright() as playwright:
        browser = playwright.chromium.connect_over_cdp(endpoint_url=browser_ws_endpoint)
        context = browser.contexts[0]
        page = context.new_page()

        # Use any Playwright command to drive the browser
        # and enjoy full protection from bot detection products
        page.goto('https://google.com')
        page.click('div[aria-modal="true"][tabindex="0"] button + button')
        page.click('[name=q]')
        page.keyboard.type('Kameleo')
        page.keyboard.press('Enter')

    # Wait for 5 seconds
    time.sleep(5)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)
except ProblemResponseException as e:
    raise Exception([str(e), json.dumps(e.error.error)])
