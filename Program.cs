using Microsoft.Identity.Client;

var app = PublicClientApplicationBuilder.Create("<Your application ID>")
    .WithTenantId("<Your tenant ID>")
    .Build();

AuthenticationResult authenticationResult = await AcquireATokenFromCacheOrDeviceCodeFlowAsync(app);
if (authenticationResult != null)
{
    Console.WriteLine(authenticationResult.ClaimsPrincipal.Claims.ToList().FirstOrDefault(x => x.Type.Equals("name"))?.Value);
    Console.ReadLine();
    Console.WriteLine(authenticationResult.AccessToken);
}

async Task<AuthenticationResult> AcquireATokenFromCacheOrDeviceCodeFlowAsync(IPublicClientApplication app)
{
    var scopes = new string[]
    {
        "offline_access",
        "openid"
    };

    AuthenticationResult result = null;
    var accounts = await app.GetAccountsAsync();
    if (accounts.Any())
    {
        try
        {
            // Attempt to get a token from the cache (or refresh it silently if needed)
            result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
        }
    }

    result = await app.AcquireTokenWithDeviceCode(
            scopes,
            deviceCodeCallback =>
            {
                // This will print the message on the console which tells the user where to go sign-in:
                Console.WriteLine(deviceCodeCallback.Message);

                return Task.FromResult(0);
            }).ExecuteAsync();

    return result;
}

