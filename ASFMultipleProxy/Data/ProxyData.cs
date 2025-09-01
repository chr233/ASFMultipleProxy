using System.Net;
using System.Text.Json.Serialization;

namespace ASFMultipleProxy.Data;
public sealed record ProxyData
{
    public string? WebProxy { get; init; }
    public string? WebProxyUsername { get; init; }
    public string? WebProxyPassword { get; init; }

    [JsonConstructor]
    private ProxyData() { }

    public WebProxy? TryCreateWebProxy()
    {
        var uri = TryParseToUri(WebProxy);
        if (uri != null)
        {
            var webProxy = new WebProxy
            {
                Address = uri,
                BypassProxyOnLocal = true
            };

            if (!string.IsNullOrEmpty(WebProxyUsername) || !string.IsNullOrEmpty(WebProxyPassword))
            {
                NetworkCredential credentials = new();

                if (!string.IsNullOrEmpty(WebProxyUsername))
                {
                    credentials.UserName = WebProxyUsername;
                }

                if (!string.IsNullOrEmpty(WebProxyPassword))
                {
                    credentials.Password = WebProxyPassword;
                }

                webProxy.Credentials = credentials;
            }
            return webProxy;
        }

        return null;
    }
}
