using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Web;
using ASFMultipleProxy.Data;
using System.Text;

namespace ASFMultipleProxy.Core;
internal static class Command
{
    /// <summary>
    /// 读取机器人代理设置
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static Task<string> ResponseGetBotProxy(Bot bot)
    {
        var handler = bot.GetHttpClientHandler();
        if (handler == null)
        {
            return Task.FromResult(bot.FormatBotResponse(Langs.InternalError));
        }

        var proxy = handler.Proxy;
        if (proxy == null)
        {
            return Task.FromResult(bot.FormatBotResponse(Langs.NotUseProxy));
        }
        else
        {
            return Task.FromResult(bot.FormatBotResponse(Langs.CurrentProxy, proxy.GetProxyAddress()));
        }
    }

    /// <summary>
    /// 读取机器人代理设置 (多个Bot)
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string> ResponseGetBotProxy(string botNames)
    {
        if (string.IsNullOrEmpty(botNames))
        {
            throw new ArgumentNullException(nameof(botNames));
        }

        var bots = Bot.GetBots(botNames);

        if (bots == null || bots.Count == 0)
        {
            return FormatStaticResponse(Strings.BotNotFound, botNames);
        }

        var results = await Utilities.InParallel(bots.Select(ResponseGetBotProxy)).ConfigureAwait(false);
        var responses = new List<string>(results.Where(static result => !string.IsNullOrEmpty(result))!);

        return string.Join(Environment.NewLine, responses);
    }

    private static async Task<IpInfoResponse?> GetIpInformation(WebBrowser? webBrowser)
    {
        if (string.IsNullOrEmpty(Config.IpInfoToken) || webBrowser == null)
        {
            return null;
        }

        var request = new Uri(IpInfoUrl, $"/?token={Config.IpInfoToken}");
        var response = await webBrowser.UrlGetToJsonObject<IpInfoResponse>(request);

        return response?.Content;
    }

    private static string ParseIpInfoResponse(string title, IpInfoResponse? response)
    {
        if (response == null)
        {
            return "Fetch Ip info failed, please check if the 'ASFEnhance.IpInfoToken' is set";
        }

        var sb = new StringBuilder();
        sb.AppendLine(title);
        sb.AppendLineFormat(Langs.TwoItem, "Ip", response.Ip);
        sb.AppendLineFormat(Langs.TwoItem, "HostName", response.Hostname);
        sb.AppendLineFormat(Langs.TwoItem, "City", response.City);
        sb.AppendLineFormat(Langs.TwoItem, "Region", response.Region);
        sb.AppendLineFormat(Langs.TwoItem, "Country", response.Country);
        sb.AppendLineFormat(Langs.TwoItem, "Location", response.Loc);
        sb.AppendLineFormat(Langs.TwoItem, "Org", response.Org);
        sb.AppendLineFormat(Langs.TwoItem, "Postal", response.Postal);
        sb.AppendLineFormat(Langs.TwoItem, "TimeZone", response.Timezone);

        return sb.ToString();
    }

    internal static async Task<string> ResponseAsfIp()
    {
        var response = await GetIpInformation(ASF.WebBrowser).ConfigureAwait(false);
        var result = ParseIpInfoResponse("Global Ip:", response);
        return FormatStaticResponse(result);
    }

    internal static async Task<string> ResponseBotIp(Bot bot)
    {
        var response = await GetIpInformation(bot.ArchiWebHandler.WebBrowser).ConfigureAwait(false);
        var result = ParseIpInfoResponse($"{bot.BotName}'s Ip:", response);
        return FormatStaticResponse(result);
    }

    internal static async Task<string> ResponseBotIp(string botNames)
    {
        if (string.IsNullOrEmpty(botNames))
        {
            throw new ArgumentNullException(nameof(botNames));
        }

        var bots = Bot.GetBots(botNames);

        if (bots == null || bots.Count == 0)
        {
            return FormatStaticResponse(Strings.BotNotFound, botNames);
        }

        var results = await Utilities.InParallel(bots.Select(ResponseBotIp)).ConfigureAwait(false);
        var responses = new List<string>(results.Where(static result => !string.IsNullOrEmpty(result))!);

        return string.Join(Environment.NewLine, responses);
    }
}

