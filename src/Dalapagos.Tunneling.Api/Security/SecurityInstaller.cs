namespace Dalapagos.Tunneling.Api.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Web;

public static class SecurityInstaller
{
    public static void AddEndpointSecurity(this IServiceCollection services, IConfiguration config)
    {
        var adConfigSection = config.GetSection("AzureAd");

        services.AddScoped<GraphServiceClient>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(adConfigSection)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph()
            .AddInMemoryTokenCaches();

        services.AddAuthorization(opt =>
        {
            var adminGroup = adConfigSection.GetValue<string>($"Groups:{Groups.TunnelingAdmin}");
            if (adminGroup != null)
            {
                opt.AddPolicy(SecurityPolicies.TunnelingAdminPolicy, policy => policy.AddRequirements(new GroupAuthorizationRequirement([adminGroup])));
            }

            var userGroup = adConfigSection.GetValue<string>($"Groups:{Groups.TunnelingUser}");
            if (userGroup != null)
            {
                opt.AddPolicy(SecurityPolicies.TunnelingUserPolicy, policy => policy.AddRequirements(new GroupAuthorizationRequirement([userGroup])));
            }
        });

        services.AddTransient<IAuthorizationHandler, GroupAuthorizationHandler>();
    }
}