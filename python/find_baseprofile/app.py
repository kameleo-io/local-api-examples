from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models.problem_response_py3 import ProblemResponseException
import time
import json


try:
    # This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
    kameleo_port = 5050

    client = KameleoLocalApiClient(f'http://localhost:{kameleo_port}')

    # Search Chrome Base Profiles
    # Possible deviceType value: desktop, mobile
    # Possible browserProduct value: chrome, firefox, edge
    # Possible osFamily values: windows, macos, linux, android, ios
    # Possible language values e.g: en-en, es,es
    # You can use empty parameters as well
    base_profiles = client.search_base_profiles(
        device_type='desktop',
        browser_product='chrome',
        os_family='macos',
        language='es-es'
    )

    # Create a new profile with recommended settings for browser fingerprinting protection
    # Choose one of the Chrome BaseProfiles
    create_profile_request = BuilderForCreateProfile \
        .for_base_profile(base_profiles[0].id) \
        .set_recommended_defaults() \
        .build()
    profile = client.create_profile(body=create_profile_request)

    # Start the browser profile
    client.start_profile(profile.id)

    # Wait for 10 seconds
    time.sleep(10)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)
except ProblemResponseException as e:
    raise Exception([str(e), json.dumps(e.error.error)])
