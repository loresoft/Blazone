namespace Sample.WebAssembly.Client.Services;

public class GatewayClient
{
    public GatewayClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public HttpClient HttpClient { get; }
}
