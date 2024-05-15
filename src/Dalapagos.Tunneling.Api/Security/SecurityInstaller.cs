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
        var adConfigSection = config.GetSection("Dalapagos-AzureAd");

        services.AddScoped<GraphServiceClient>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(adConfigSection)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph()
            .AddInMemoryTokenCaches();

        services.AddAuthorization(opt =>
        {
            var rportAdminGroup = adConfigSection.GetValue<string>($"Groups:{Groups.RportAdmin}");
            if (rportAdminGroup != null)
            {
                opt.AddPolicy(Policies.RportAdminPolicy, policy => policy.AddRequirements(new GroupAuthorizationRequirement([rportAdminGroup])));
            }
        });

        services.AddTransient<IAuthorizationHandler, GroupAuthorizationHandler>();
    }
}