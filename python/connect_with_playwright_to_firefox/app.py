from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from playwright.sync_api import sync_playwright
import time
from os import path, getenv
from platform import system
import os

# This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
kameleo_port = os.getenv('KAMELEO_PORT', '5050')

client = KameleoLocalApiClient(
    endpoint=f'http://localhost:{kameleo_port}',
    retry_total=0
)

# Search Firefox Base Profiles
base_profiles = client.search_base_profiles(
    device_type='desktop',
    browser_product='firefox'
)

# Create a new profile with recommended settings
# Choose one of the Base Profiles
create_profile_request = BuilderForCreateProfile \
    .for_base_profile(base_profiles[0].id) \
    .set_name('connect with Playwright to Firefox example') \
    .set_recommended_defaults() \
    .build()
profile = client.create_profile(body=create_profile_request)

# Start the Kameleo profile and connect with Playwright
browser_ws_endpoint = f'ws://localhost:{kameleo_port}/playwright/{profile.id}'
with sync_playwright() as playwright:
    # The Playwright framework is not designed to connect to already running
    # browsers. To overcome this limitation, a tool bundled with Kameleo, named
    # pw-bridge.exe will bridge the communication gap between the running Firefox
    # instance and this playwright script.
    # The exact path to the bridge executable is subject to change
    pw_bridge_path = getenv('PW_BRIDGE_PATH')
    if pw_bridge_path == None and system() == 'Windows':
        pw_bridge_path = path.expandvars(r'%LOCALAPPDATA%\Programs\Kameleo\pw-bridge.exe')
    elif pw_bridge_path == None and system() == 'Darwin':
        pw_bridge_path = '/Applications/Kameleo.app/Contents/MacOS/pw-bridge'
    browser = playwright.firefox.launch_persistent_context(
        '',
        executable_path=pw_bridge_path,
        args=[f'-target {browser_ws_endpoint}'],
        viewport=None)

    # Kameleo will open the a new page in the default browser context.
    # NOTE: We DO NOT recommend using multiple browser contexts, as this might interfere
    #       with Kameleo's browser fingerprint modification features.
    page = browser.new_page()

    # Use any Playwright command to drive the browser
    # and enjoy full protection from bot detection products
    page.goto('https://wikipedia.org')
    page.click('[name=search]')
    page.keyboard.type('Chameleon')
    page.keyboard.press('Enter')

    # Wait for 5 seconds
    time.sleep(5)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)
