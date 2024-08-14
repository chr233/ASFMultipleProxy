using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;

namespace ASFMultipleProxy.Core;
internal static class Command
{
    /// <summary>
    /// 读取机器人代理设置
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static Task<string?> ResponseGetBotProxy(Bot bot)
    {
        var handler = bot.GetHttpClientHandler();
        if (handler == null)
        {
            return Task.FromResult<string?>(bot.FormatBotResponse(Langs.InternalError));
        }

        var proxy = handler.Proxy;
        if (proxy == null)
        {
            return Task.FromResult<string?>(bot.FormatBotResponse("Langs.NotUseProxy"));
        }
        else
        {
            return Task.FromResult<string?>(bot.FormatBotResponse(Langs.CurrentProxy, proxy.GetProxyAddress()));
        }
    }

    /// <summary>
    /// 读取机器人代理设置 (多个Bot)
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string?> ResponseGetBotProxy(string botNames)
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

        var results = await Utilities.InParallel(bots.Select(bot => ResponseGetBotProxy(bot))).ConfigureAwait(false);
        var responses = new List<string>(results.Where(static result => !string.IsNullOrEmpty(result))!);

        return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
    }
}

