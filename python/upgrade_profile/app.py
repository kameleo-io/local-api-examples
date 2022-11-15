from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models.problem_response_py3 import ProblemResponseException
import time
import json


try:
    kameleo_port = 5050

    client = KameleoLocalApiClient(f'http://localhost:{kameleo_port}')

    # Search for a Desktop Base Profile with Windows OS and Chrome browser
    base_profiles = client.search_base_profiles(
        device_type='desktop',
        os_family='windows',
        browser_product='chrome'
    )

    # Find a Base Profile with the oldest available version of chrome
    base_profile = sorted(base_profiles, key=lambda x: x.browser.major)[0]

    # Create a new profile with recommended settings
    # Choose one of the Base Profiles
    create_profile_request = BuilderForCreateProfile \
        .for_base_profile(base_profile.id) \
        .set_recommended_defaults() \
        .build()
    profile = client.create_profile(body=create_profile_request)

    print(f'Profile\'s browser before update is {profile.base_profile.browser.product} {profile.base_profile.browser.version}')

    # The Base Profile’s browser version will be updated if there is any available on our servers
    profile = client.upgrade_profile(profile.id)
    print(f'Profile\'s browser after update is {profile.base_profile.browser.product} {profile.base_profile.browser.version}')

    # Start the browser profile
    client.start_profile(profile.id)

    # Wait for 5 seconds
    time.sleep(5)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)
except ProblemResponseException as e:
    raise Exception([str(e), json.dumps(e.error.error)])
