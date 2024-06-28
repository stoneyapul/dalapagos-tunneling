namespace Dalapagos.Tunneling.Api.Security;

using Hangfire.Annotations;
using Hangfire.Dashboard;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        // TODO: Implement Hangfire authorization.
        return true;    
    }
}
