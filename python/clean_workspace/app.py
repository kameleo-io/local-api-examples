from kameleo.local_api_client import KameleoLocalApiClient


# This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
kameleo_port = 5050

client = KameleoLocalApiClient(
    endpoint=f'http://localhost:{kameleo_port}',
    retry_total=0
)

profiles = client.list_profiles()
for profile in profiles:
    client.delete_profile(profile.id)

print(f'{len(profiles)} profiles deleted.')
