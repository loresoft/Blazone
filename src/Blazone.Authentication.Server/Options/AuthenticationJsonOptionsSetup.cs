using Blazone.Authentication.Http;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Blazone.Authentication.Options;

internal sealed class AuthenticationJsonOptionsSetup : IConfigureOptions<JsonOptions>
{
    public void Configure(JsonOptions options)
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AuthenticationJsonContext.Default);
    }
}
