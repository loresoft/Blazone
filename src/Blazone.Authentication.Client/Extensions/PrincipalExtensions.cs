using System.Security.Claims;
using System.Security.Principal;

namespace Blazone.Authentication.Extensions;

public static class PrincipalExtensions
{
    private const string ObjectIdenttifier = "oid";

    private const string Subject = "sub";
    private const string NameClaim = "name";
    private const string EmailClaim = "email";
    private const string EmailsClaim = "emails";
    private const string ProviderClaim = "idp";

    private const string IdentityClaim = "http://schemas.microsoft.com/identity/claims/identityprovider";
    private const string IdentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public static string? GetEmail(this IPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentNullException(nameof(principal));

        var claimPrincipal = principal as ClaimsPrincipal;
        var claim = claimPrincipal?.FindFirst(ClaimTypes.Email)
            ?? claimPrincipal?.FindFirst(EmailClaim)
            ?? claimPrincipal?.FindFirst(EmailsClaim);

        return claim?.Value;
    }

    public static Guid? GetObjectId(this IPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentNullException(nameof(principal));

        var claimPrincipal = principal as ClaimsPrincipal;
        var claim = claimPrincipal?.FindFirst(IdentifierClaim)
            ?? claimPrincipal?.FindFirst(ObjectIdenttifier)
            ?? claimPrincipal?.FindFirst(ClaimTypes.NameIdentifier);

        return Guid.TryParse(claim?.Value, out var oid) ? oid : null;
    }

    public static string? GetName(this IPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentNullException(nameof(principal));

        var claimPrincipal = principal as ClaimsPrincipal;
        var claim = claimPrincipal?.FindFirst(NameClaim)
            ?? claimPrincipal?.FindFirst(ClaimTypes.Name)
            ?? claimPrincipal?.FindFirst(Subject);

        return claim?.Value;
    }

    public static string? GetProvider(this IPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentNullException(nameof(principal));

        var claimPrincipal = principal as ClaimsPrincipal;
        var claim = claimPrincipal?.FindFirst(ProviderClaim)
            ?? claimPrincipal?.FindFirst(IdentityClaim);

        return claim?.Value;
    }

}
