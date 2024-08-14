using ArchiSteamFarm.Core;
using ArchiSteamFarm.Helpers.Json;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Web.GitHub.Data;
using ASFMultipleProxy.Core;
using ASFMultipleProxy.Data;

using System.ComponentModel;
using System.Composition;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ASFMultipleProxy;

[Export(typeof(IPlugin))]
internal class ASFMultipleProxy : IASF, IBot, IBotModules, IBotCommand2, IGitHubPluginUpdates
{
    public string Name => "ASF Multiple Proxy";
    public Version Version => MyVersion;

    public bool CanUpdate => true;
    public string RepositoryName => "chr233/ASFMultipleProxy";

    private bool ASFEBridge;

    private Timer? StatisticTimer { get; set; }

    /// <inheritdoc/>
    public Task OnASFInit(IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null)
    {
        PluginConfig? config = null;

        if (additionalConfigProperties != null)
        {
            foreach (var (configProperty, configValue) in additionalConfigProperties)
            {
                if (configProperty == "ASFEnhance" && configValue.ValueKind == JsonValueKind.Object)
                {
                    try
                    {
                        config = configValue.ToJsonObject<PluginConfig>();
                        if (config != null)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ASFLogger.LogGenericException(ex);
                    }
                }
            }
        }

        Utils.Config = config ?? new();

        var sb = new StringBuilder();

        //使用协议
        if (!Config.EULA)
        {
            sb.AppendLine(Langs.Line);
            sb.AppendLineFormat(Langs.EulaWarning, Name);
            sb.AppendLine(Langs.Line);
        }

        if (sb.Length > 0)
        {
            sb.Insert(0, "\r\n");
            ASFLogger.LogGenericWarning(sb.ToString());
        }

        if (!Config.EULA)
        {
            return Task.CompletedTask;
        }

        // 创建代理
        if (Config.MultWebProxy?.Count > 0)
        {
            foreach (var proxy in Config.MultWebProxy)
            {
                var webProxy = proxy.TryCreateWebProxy();
                if (webProxy != null)
                {
                    AvilableProxies.Add(webProxy);
                }
            }
        }

        if (AvilableProxies.Count > 0)
        {
            ASFLogger.LogGenericInfo(string.Format(Langs.MultProxyLoaded, AvilableProxies.Count));
        }
        else
        {
            ASFLogger.LogGenericInfo(Langs.MultProxyDoNotSet);
        }

        //统计
        if (Config.Statistic && !ASFEBridge)
        {
            var request = new Uri("https://asfe.chrxw.com/ASFAwardTool");
            StatisticTimer = new Timer(
                async (_) =>
                {
                    await ASF.WebBrowser!.UrlGetToHtmlDocument(request).ConfigureAwait(false);
                },
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromHours(24)
            );
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnLoaded()
    {
        ASFLogger.LogGenericInfo(Langs.PluginContact);
        ASFLogger.LogGenericInfo(Langs.PluginInfo);

        var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var handler = typeof(ASFMultipleProxy).GetMethod(nameof(ResponseCommand), flag);

        const string pluginId = nameof(ASFMultipleProxy);
        const string cmdPrefix = "AMP";
        const string? repoName = null;

        ASFEBridge = AdapterBridge.InitAdapter(Name, pluginId, cmdPrefix, repoName, handler);

        if (ASFEBridge)
        {
            ASFLogger.LogGenericDebug(Langs.ASFEnhanceRegisterSuccess);
        }
        else
        {
            ASFLogger.LogGenericInfo(Langs.ASFEnhanceRegisterFailed);
            ASFLogger.LogGenericWarning(Langs.PluginStandalongMode);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取插件信息
    /// </summary>
    private static string? PluginInfo => string.Format("{0} {1}", nameof(ASFMultipleProxy), MyVersion);

    /// <summary>
    /// 处理命令
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="cmd"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Task<string?>? ResponseCommand(Bot bot, EAccess access, string cmd, string[] args)
    {
        int argLength = args.Length;

        return argLength switch
        {
            0 => throw new InvalidOperationException(nameof(args)),
            1 => cmd switch  //不带参数
            {
                //Plugin Info
                "ASFMULTIPLEPROXY" or
                "AMP" when access >= EAccess.FamilySharing =>
                    Task.FromResult(PluginInfo),

                "GETPROXY" when access >= EAccess.Master =>
                    Command.ResponseGetBotProxy(bot),

                _ => null,
            },
            _ => cmd switch //带参数
            {
                "GETPROXY" when access >= EAccess.Master =>
                    Command.ResponseGetBotProxy(Utilities.GetArgsAsText(args, 1, ",")),

                _ => null,
            }
        };
    }

    /// <inheritdoc/>
    public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamId = 0)
    {
        if (ASFEBridge || !Config.EULA)
        {
            return null;
        }

        if (!Enum.IsDefined(access))
        {
            throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
        }

        try
        {
            var cmd = args[0].ToUpperInvariant();

            if (cmd.StartsWith("AAT."))
            {
                cmd = cmd[4..];
            }

            var task = ResponseCommand(bot, access, cmd, args);
            if (task != null)
            {
                return await task.ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                ASFLogger.LogGenericException(ex);
            }).ConfigureAwait(false);

            return ex.StackTrace;
        }
    }

    /// <inheritdoc/>
    public Task OnBotDestroy(Bot bot)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBotInit(Bot bot)
    {
        var proxy = GetRandomProxy();

        var msg = bot.FormatBotResponse(proxy == null ? Langs.NoAvilableProxy : SetNewProxy(bot, proxy));
        ASFLogger.LogGenericWarning(msg);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBotInitModules(Bot bot, IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties)
    {
        if (additionalConfigProperties != null)
        {
            foreach (var (configProperty, configValue) in additionalConfigProperties)
            {
                if (configProperty == "WebProxy" && configValue.ValueKind == JsonValueKind.Object)
                {
                    try
                    {
                        var proxy = configValue.ToJsonObject<ProxyData>();
                        var webProxy = proxy?.TryCreateWebProxy();
                        if (webProxy != null)
                        {
                            var msg = bot.FormatBotResponse(SetNewProxy(bot, webProxy));
                            ASFLogger.LogGenericWarning(msg);

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ASFLogger.LogGenericException(ex);
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ReleaseAsset?> GetTargetReleaseAsset(Version asfVersion, string asfVariant, Version newPluginVersion, IReadOnlyCollection<ReleaseAsset> releaseAssets)
    {
        var result = releaseAssets.Count switch
        {
            0 => null,
            1 => //如果找到一个文件，则第一个
                releaseAssets.First(),
            _ => //优先下载当前语言的版本
                releaseAssets.FirstOrDefault(static x => x.Name.Contains(Langs.CurrentLanguage)) ??
                releaseAssets.FirstOrDefault(static x => x.Name.Contains("en-US"))
        };

        return Task.FromResult(result);
    }
}
