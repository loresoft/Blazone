using Blazone.Authentication.Options;

namespace Blazone.Authentication.Tests.Options;

public class AuthenticationEndpointOptionsTests
{
    [Fact]
    public void SignInUrlTest()
    {
        var options = new AuthenticationEndpointOptions();

        options.SignInUrl().Should().Be("/blazone/signin?returnUrl=%2F");
        options.SignInUrl("/test").Should().Be("/blazone/signin?returnUrl=%2Ftest");
        options.SignInUrl("test").Should().Be("/blazone/signin?returnUrl=test");

        options.RoutePrefix = "auth";
        options.SignInUrl().Should().Be("/auth/signin?returnUrl=%2F");

        options.RoutePrefix = "a";
        options.SignInRoute = "login";
        options.SignInUrl().Should().Be("/a/login?returnUrl=%2F");
    }

    [Fact]
    public void SignOutUrlTest()
    {
        var options = new AuthenticationEndpointOptions();

        options.SignOutUrl().Should().Be("/blazone/signout?returnUrl=%2F");
        options.SignOutUrl("/test").Should().Be("/blazone/signout?returnUrl=%2Ftest");
        options.SignOutUrl("test").Should().Be("/blazone/signout?returnUrl=test");

        options.RoutePrefix = "auth";
        options.SignOutUrl().Should().Be("/auth/signout?returnUrl=%2F");

        options.RoutePrefix = "a";
        options.SignOutRoute = "logout";
        options.SignOutUrl().Should().Be("/a/logout?returnUrl=%2F");
    }

    [Fact]
    public void UserUrlTest()
    {
        var options = new AuthenticationEndpointOptions();

        options.UserUrl().Should().Be("/blazone/user");

        options.RoutePrefix = "auth";
        options.UserUrl().Should().Be("/auth/user");

        options.RoutePrefix = "a";
        options.UserRoute = "me";
        options.UserUrl().Should().Be("/a/me");
    }
}
