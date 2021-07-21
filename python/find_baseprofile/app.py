from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
import time


kameleoBaseUrl = 'http://localhost:5050'
client = KameleoLocalApiClient(kameleoBaseUrl)

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
