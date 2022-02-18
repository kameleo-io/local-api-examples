const { KameleoLocalApiClient, BuilderForCreateProfile } = require('@kameleo/local-api-client');

(async () => {
    try {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        const kameleoPort = 5050;

        const client = new KameleoLocalApiClient({
            baseUri: `http://localhost:${kameleoPort}`,
            noRetryPolicy: true,
        });

        // Search Chrome Base Profiles
        const testProxyResult = await client.testProxy({
            body: {
                value: 'http',
                extra: {
                    host: '<host>',
                    port: 1080,
                    id: '<username>',
                    secret: '<password>',
                },
            },
        });
        console.log('Test proxy result', testProxyResult);
        
    } catch (error) {
        console.error(error);
    }
})();
