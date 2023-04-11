from kameleo.local_api_client import KameleoLocalApiClient
from kameleo.local_api_client.models import Server


# This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
kameleo_port = 5050

client = KameleoLocalApiClient(
    endpoint=f'http://localhost:{kameleo_port}',
    retry_total=0
)

# Create test proxy request object
request = {'value': 'http', 'extra': Server(host='<host>', port=1080, id='<userId>', secret='<userSecret>')}

# Search Chrome Base Profiles
test_proxy_response = client.test_proxy(body=request)
print('Result: ' + str(test_proxy_response))
