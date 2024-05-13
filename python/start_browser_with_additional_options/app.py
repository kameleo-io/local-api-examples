from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
import time

from kameleo.local_api_client.models import WebDriverSettings, Preference


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
    .set_name('start browser with additional options example') \
    .set_recommended_defaults() \
    .build()
profile = client.create_profile(body=create_profile_request)

# Provide additional settings for the web driver when starting the browser
# Use this command to customize the browser process by adding command-line arguments
#  like '--mute-audio' or '--start-maximized'
#  or modify the native profile settings when starting the browser

# start the browser with the --mute-audio command line argument
client.start_profile_with_options(profile.id, WebDriverSettings(
    arguments=['mute-audio'],
))
# Wait for 10 seconds
time.sleep(10)
# Stop the profile
client.stop_profile(profile.id)

# start the browser with an additional Selenum option
client.start_profile_with_options(profile.id, WebDriverSettings(
    additional_options=[
        Preference(key='pageLoadStrategy', value='eager'),
    ]
))
time.sleep(10)
client.stop_profile(profile.id)

# start the browser and also set a Chrome preference
client.start_profile_with_options(profile.id, WebDriverSettings(
    preferences=[
        Preference(key='profile.managed_default_content_settings.images', value=2),
    ]
)) 
time.sleep(10)
client.stop_profile(profile.id)
