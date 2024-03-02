using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;

namespace Sample.WebAssembly.Server.Services;

public class ClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var clone = principal.Clone();
        var identity = clone.Identity as ClaimsIdentity;

        identity?.AddClaim(new Claim(ClaimTypes.PostalCode, "55346"));

        return Task.FromResult(clone);
    }
}
