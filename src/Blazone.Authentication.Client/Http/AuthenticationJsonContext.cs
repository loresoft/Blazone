using System.Text.Json.Serialization;

using Blazone.Authentication.Models;

namespace Blazone.Authentication.Http;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ClaimValue))]
[JsonSerializable(typeof(UserClaim))]
public partial class AuthenticationJsonContext : JsonSerializerContext;
