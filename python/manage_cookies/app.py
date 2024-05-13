from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models import CookieRequest
from selenium import webdriver
import time


# This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
kameleo_port = 5050

client = KameleoLocalApiClient(
    endpoint=f'http://localhost:{kameleo_port}',
    retry_total=0
)

# Search Firefox Base Profiles
base_profiles = client.search_base_profiles(
    device_type='desktop',
    browser_product='firefox'
)

# Create a new profile with recommended settings for browser fingerprinting protection
# Choose one of the Firefox BaseProfiles
create_profile_request = BuilderForCreateProfile \
    .for_base_profile(base_profiles[0].id) \
    .set_name('manage cookies example') \
    .set_recommended_defaults() \
    .build()
profile = client.create_profile(body=create_profile_request)

# Start the Kameleo profile and connect using WebDriver protocol
options = webdriver.ChromeOptions()
options.add_experimental_option('kameleo:profileId', profile.id)
driver = webdriver.Remote(
    command_executor=f'http://localhost:{kameleo_port}/webdriver',
    options=options
)

# Navigate to a site which give you cookies
driver.get('https://whoer.net')
time.sleep(5)

driver.get('https://youtube.com')
time.sleep(5)

driver.get('https://www.nytimes.com')
time.sleep(5)

# Stop the browser by stopping the Kameleo profile
client.stop_profile(profile.id)

# You can list all of your cookies
cookie_list = client.list_cookies(profile.id)
print(f'There are {len(cookie_list)} cookies in the profile')

# You can modify cookie or you can add new
cookie = cookie_list[0]
new_cookie = CookieRequest(domain=cookie.domain, name=cookie.name, path=cookie.path, value=cookie.value,
                           host_only=cookie.host_only, http_only=cookie.http_only, secure=cookie.secure,
                           same_site=cookie.same_site, expiration_date=cookie.expiration_date)
cookie_array = [new_cookie]
client.add_cookies(profile.id, body=cookie_array)

# You can delete all cookies of the profile
client.delete_cookies(profile.id)
