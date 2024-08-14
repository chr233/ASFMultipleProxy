using System.Net;

namespace ASFMultipleProxy.Data;
public sealed record ProxyData
{
    public string? WebProxy { get; init; }
    public string? WebProxyUsername { get; init; }
    public string? WebProxyPassword { get; init; }

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
