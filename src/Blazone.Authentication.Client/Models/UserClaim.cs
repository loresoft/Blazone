namespace Blazone.Authentication.Models;

public record UserClaim(
    bool IsAuthenticated,
    string? NameClaimType = null,
    string? RoleClaimType = null,
    IReadOnlyCollection<ClaimValue>? Claims = null);
