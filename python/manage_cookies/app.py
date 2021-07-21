from kameleo.local_api_client.kameleo_local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from kameleo.local_api_client.models.cookie_request_py3 import CookieRequest
from selenium import webdriver
import time


kameleoBaseUrl = 'http://localhost:5050'
client = KameleoLocalApiClient(kameleoBaseUrl)

# Search Chrome Base Profiles
base_profiles = client.search_base_profiles(
    device_type='desktop',
    browser_product='firefox'
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

# Connect to the running browser instance using WebDriver
options = webdriver.ChromeOptions()
options.add_experimental_option("kameleo:profileId", profile.id)
driver = webdriver.Remote(
    command_executor=f'{kameleoBaseUrl}/webdriver',
    options=options
)

# Navigate to a site which give you cookies
driver.get('https://whoer.net')
time.sleep(15)

driver.get('https://youtube.com')
time.sleep(15)

driver.get('https://translate.google.com')
time.sleep(15)

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
