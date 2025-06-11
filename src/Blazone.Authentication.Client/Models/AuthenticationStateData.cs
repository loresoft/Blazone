using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

namespace Blazone.Authentication.Models;

/// <summary>
/// A JSON-serializable type that represents the data that is used to create an <see cref="AuthenticationState"/>.
/// </summary>
public class AuthenticationStateData
{
    /// <summary>
    /// The client-readable claims that describe the <see cref="AuthenticationState.User"/>.
    /// </summary>
    public IList<ClaimData> Claims { get; set; } = [];

    /// <summary>
    /// Gets the value that identifies 'Name' claims. This is used when returning the property <see cref="ClaimsIdentity.Name"/>.
    /// </summary>
    public string NameClaimType { get; set; } = ClaimsIdentity.DefaultNameClaimType;

    /// <summary>
    /// Gets the value that identifies 'Role' claims. This is used when calling <see cref="ClaimsPrincipal.IsInRole"/>.
    /// </summary>
    public string RoleClaimType { get; set; } = ClaimsIdentity.DefaultRoleClaimType;
}
