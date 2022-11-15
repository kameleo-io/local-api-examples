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

    # Start the browser profile
    client.start_profile(profile.id)

    # Wait for 5 seconds
    time.sleep(5)

    # Stop the browser by stopping the Kameleo profile
    client.stop_profile(profile.id)

    # The duplicated profile is in the memory only and will be deleted when the Kameleo.CLI is closed unless you save it.
    duplicatedProfile = client.duplicate_profile(profile.id)
    print(f'Profile {duplicatedProfile.name} is just created')
except ProblemResponseException as e:
    raise Exception([str(e), json.dumps(e.error.error)])
