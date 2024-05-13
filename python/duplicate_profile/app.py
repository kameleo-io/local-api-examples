from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
import time


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
    .set_name('duplicate profile example') \
    .set_recommended_defaults() \
    .build()
profile = client.create_profile(body=create_profile_request)

# The duplicated profile is in the memory only and will be deleted when the Kameleo.CLI is closed unless you export it.
duplicatedProfile = client.duplicate_profile(profile.id)
print(f'Profile {duplicatedProfile.name} is just created')
