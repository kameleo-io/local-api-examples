from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
import time
import os

from kameleo.local_api_client.models import ExportProfileRequest, ImportProfileRequest


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

# Create a new profile with recommended settings for browser fingerprinting protection
# Choose one of the Chrome BaseProfiles
create_profile_request = BuilderForCreateProfile \
    .for_base_profile(base_profiles[0].id) \
    .set_recommended_defaults() \
    .build()
profile = client.create_profile(body=create_profile_request)

# Export the profile to a given path
folder = os.path.dirname(os.path.realpath(__file__))
path = f'{folder}\\test.kameleo'
result = client.export_profile(profile.id, body=ExportProfileRequest(path=path))
print(f'Profile has been exported to {folder}')

# You have to delete this profile if you want to import back
client.delete_profile(profile.id);

# Import the profile from the given url
profile = client.import_profile(body=ImportProfileRequest(path=path))

# Start the profile
client.start_profile(profile.id)

# Wait for 5 seconds
time.sleep(5)

# Stop the browser by stopping the Kameleo profile
client.stop_profile(profile.id)
