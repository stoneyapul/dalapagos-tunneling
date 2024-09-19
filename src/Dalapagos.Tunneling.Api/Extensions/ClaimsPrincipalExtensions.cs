namespace Dalapagos.Tunneling.Api.Extensions;

using System.Security.Claims;
using Core.Exceptions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        if (Guid.TryParse(userId, out var userIdGuid))
        {
            return userIdGuid;
        }

        throw new AccessDeniedException();
    }
}
