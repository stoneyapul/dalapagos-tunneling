using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text.Json;

var config = new PublicClientApplicationOptions
{
    TenantId = "13cfba4f-9203-4496-a424-b19fe4f06252",
    ClientId = "5d91e02e-552b-4968-aea4-d153fcd116a1"
};

IPublicClientApplication publicMsalClient = PublicClientApplicationBuilder.CreateWithApplicationOptions(config)
                                                                          .Build();
AuthenticationResult? msalAuthenticationResult = null;

// Attempt to use a cached access token if one is available. This will renew existing, but
// expired access tokens if possible. In this specific sample, this will always result in
// a cache miss, but this pattern would be what you'd use on subsequent calls that require
// the usage of the same access token.
IEnumerable<IAccount> accounts = (await publicMsalClient.GetAccountsAsync()).ToList();

if (accounts.Any())
{
    try
    {
        msalAuthenticationResult = await publicMsalClient.AcquireTokenSilent([], accounts.First()).ExecuteAsync();
    }
    catch (MsalUiRequiredException)
    {
        // No usable cached token was found for this scope + account or Azure AD insists in
        // an interactive user flow.
    }
}

if (msalAuthenticationResult == null)
{
    // Initiate the device code flow.
    msalAuthenticationResult = await publicMsalClient.AcquireTokenWithDeviceCode([], deviceCodeResultCallback =>
    {
        // This will print the message on the console which tells the user where to go sign-in using
        // a separate browser and the code to enter once they sign in.
        // The AcquireTokenWithDeviceCode() method will poll the server after firing this
        // device code callback to look for the successful login of the user via that browser.
        Console.WriteLine(deviceCodeResultCallback.Message);
        return Task.CompletedTask;
    }).ExecuteAsync();
}

Console.WriteLine($"Access token: {msalAuthenticationResult.AccessToken}");
Console.ReadLine();