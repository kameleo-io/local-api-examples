from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.builder_for_create_profile import BuilderForCreateProfile
from selenium import webdriver
import time
import os
import random


# This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
kameleo_port = os.getenv('KAMELEO_PORT', '5050')

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
    .set_name('cookie robot example') \
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

allSites = ["instagram.com",
    "linkedin.com",
    "ebay.com",
    "pinterest.com",
    "reddit.com",
    "cnn.com",
    "bbc.co.uk",
    "nytimes.com",
    "reuters.com",
    "theguardian.com",
    "foxnews.com"]

# Select sites to collect cookies from
sitesToVisit = []
while len(sitesToVisit) < 5:
    site = allSites[random.randint(0, len(allSites) - 1)]
    if not site in set(sitesToVisit) :
        sitesToVisit.append(site)

# Warm up profile by visiting the randomly selected sites
for site in sitesToVisit:
    driver.get(f'https://{site}')
    time.sleep(random.randint(5, 15))

# Stop the browser by stopping the Kameleo profile
client.stop_profile(profile.id)
