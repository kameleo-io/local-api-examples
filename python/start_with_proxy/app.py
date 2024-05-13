from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models import Server
import time
import os


# This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
kameleo_port = 5050

PROXY_HOST = os.getenv('PROXY_HOST', 'your proxy host')
PROXY_PORT = os.getenv('PROXY_PORT', 'your proxy port')
PROXY_USERNAME = os.getenv('PROXY_USERNAME', 'your proxy username')
PROXY_PASSWORD = os.getenv('PROXY_PASSWORD', 'your proxy password')

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
# You can set your proxy up in the set_proxy method
create_profile_request = BuilderForCreateProfile \
    .for_base_profile(base_profiles[0].id) \
    .set_name('start with proxy example') \
    .set_recommended_defaults() \
    .set_proxy('socks5', Server(host=PROXY_HOST, port=PROXY_PORT, id=PROXY_USERNAME, secret=PROXY_PASSWORD)) \
    .build()
profile = client.create_profile(body=create_profile_request)

# Start the browser profile
client.start_profile(profile.id)

# Wait for 10 seconds
time.sleep(10)

# Stop the browser by stopping the Kameleo profile
client.stop_profile(profile.id)
