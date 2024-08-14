using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Web;

using ASFMultipleProxy.Data;

using System.Net;
using System.Reflection;
using System.Text;

using static ArchiSteamFarm.Steam.Integration.ArchiWebHandler;

namespace ASFMultipleProxy;

internal static class Utils
{
    /// <summary>
    /// 插件配置
    /// </summary>
    internal static PluginConfig Config { get; set; } = new();

    /// <summary>
    /// 格式化返回文本
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string FormatStaticResponse(string message)
    {
        return $"<ASFE> {message}";
    }

    /// <summary>
    /// 格式化返回文本
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static string FormatStaticResponse(string message, params object?[] args)
    {
        return FormatStaticResponse(string.Format(message, args));
    }

    /// <summary>
    /// 格式化返回文本
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string FormatBotResponse(this Bot bot, string message)
    {
        return $"<{bot.BotName}> {message}";
    }

    /// <summary>
    /// 格式化返回文本
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static string FormatBotResponse(this Bot bot, string message, params object?[] args)
    {
        return bot.FormatBotResponse(string.Format(message, args));
    }

    internal static StringBuilder AppendLineFormat(this StringBuilder sb, string format, params object?[] args)
    {
        return sb.AppendLine(string.Format(format, args));
    }

    /// <summary>
    /// 可用代理列表
    /// </summary>
    internal static List<IWebProxy> AvilableProxies { get; } = [];

    /// <summary>
    /// 获取随机代理
    /// </summary>
    /// <returns></returns>
    internal static IWebProxy? GetRandomProxy()
    {
        if (AvilableProxies.Count == 0)
        {
            return null;
        }

        var index = Random.Shared.Next(0, AvilableProxies.Count);
        return AvilableProxies[index];
    }

    /// <summary>
    /// 解析为Uri
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    internal static Uri? TryParseToUri(string? uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return null;
        }

        try
        {
            return new Uri(uri);
        }
        catch (UriFormatException e)
        {
            ASFLogger.LogGenericException(e);
            return null;
        }
    }

    /// <summary>
    /// 使用反射获取 HttpClientHandler
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal static HttpClientHandler? GetHttpClientHandler(this Bot bot)
    {
        var webBrowser = bot.ArchiWebHandler.WebBrowser;
        var fieldInfo = typeof(WebBrowser).GetField("HttpClientHandler", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            var httpClientHandler = fieldInfo.GetValue(webBrowser) as HttpClientHandler;
            return httpClientHandler;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 设置新代理
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    internal static string SetNewProxy(Bot bot, IWebProxy proxy)
    {
        var handler = bot.GetHttpClientHandler();
        if (handler == null)
        {
            return Langs.InternalError;
        }

        handler.Proxy = proxy;
        handler.UseProxy = true;

        return string.Format(Langs.ApplyNewProxy, proxy.GetProxyAddress());
    }

    /// <summary>
    /// 获取版本号
    /// </summary>
    internal static Version MyVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0.0");

    /// <summary>
    /// 获取ASF版本
    /// </summary>
    internal static Version ASFVersion => typeof(ASF).Assembly.GetName().Version ?? new Version("0.0.0.0");

    /// <summary>
    /// 日志
    /// </summary>
    internal static ArchiLogger ASFLogger => ASF.ArchiLogger;

    /// <summary>
    /// 逗号分隔符
    /// </summary>
    internal static readonly char[] SeparatorDot = [','];

    /// <summary>
    /// 加号分隔符
    /// </summary>
    internal static readonly char[] SeparatorPlus = ['+'];

    /// <summary>
    /// 逗号空格分隔符
    /// </summary>
    internal static readonly char[] SeparatorDotSpace = [',', ' '];

    internal static string? GetProxyAddress(this IWebProxy proxy)
    {
        return proxy.GetProxy(SteamStoreURL)?.ToString();
    }
}
