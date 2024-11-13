from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models import WebglMetaSpoofingOptions
import time
import os


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
    .set_name('create profile example') \
    .set_recommended_defaults() \
    .set_webgl('noise') \
    .set_webgl_meta('manual', WebglMetaSpoofingOptions(vendor='Google Inc.',
                                                       renderer='ANGLE (Intel(R) HD Graphics 630 Direct3D11 vs_5_0 ps_5_0)')) \
    .set_start_page('https://kameleo.io') \
    .set_password_manager('enabled') \
    .build()
profile = client.create_profile(body=create_profile_request)

# Start the browser profile
client.start_profile(profile.id)

# Wait for 10 seconds
time.sleep(10)

# Stop the browser by stopping the Kameleo profile
client.stop_profile(profile.id)

# Duplicate the previously created profile
duplicatedProfile = client.duplicate_profile(profile.id)
print(f'Profile {duplicatedProfile.name} is created')

# Change every property that you want to update
duplicatedProfile.name = 'duplicate profile example'
duplicatedProfile.webgl_meta.value = 'automatic'

# Send the update request and the response will be your updated profile
duplicatedProfile = client.update_profile(duplicatedProfile.id, body=duplicatedProfile)

# Start the duplicated browser profile
client.start_profile(duplicatedProfile.id)

# Wait for 10 seconds
time.sleep(10)

# Stop the browser by stopping the Kameleo profile
client.stop_profile(duplicatedProfile.id)

# Delete original profile
# Profiles need to be deleted explicitly becase they are persisted so they are available after restarting Kameleo
client.delete_profile(profile.id)
