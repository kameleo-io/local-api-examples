from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models.web_driver_settings_py3 import WebDriverSettings
from kameleo.local_api_client.models.preference_py3 import Preference
from kameleo.local_api_client.models.problem_response_py3 import ProblemResponseException
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

    # Create a new profile with recommended settings for browser fingerprinting protection
    # Choose one of the Chrome BaseProfiles
    create_profile_request = BuilderForCreateProfile \
        .for_base_profile(base_profiles[0].id) \
        .set_recommended_defaults() \
        .build()
    profile = client.create_profile(body=create_profile_request)

    # Provide additional settings for the web driver when starting the browser
    client.start_profile_with_web_driver_settings(profile.id, WebDriverSettings(
        arguments=["mute-audio"],
        preferences=[
            Preference(key='profile.managed_default_content_settings.images', value=2),
        ]
    ))

    # Wait for 10 seconds
    time.sleep(10)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)
except ProblemResponseException as e:
    raise Exception([str(e), json.dumps(e.error.error)])