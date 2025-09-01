using System.Text.Json.Serialization;

namespace ASFMultipleProxy.Data;

/// <summary>
/// 插件配置
/// </summary>
public record PluginConfig
{
    /// <summary>
    /// 是否同意使用协议
    /// </summary>
    [JsonPropertyName("EULA")]
    public bool EULA { get; set; }
    /// <summary>
    /// 启用统计信息
    /// </summary>
    [JsonPropertyName("Statistic")]
    public bool Statistic { get; set; }
    /// <summary>
    /// 多个代理地址
    /// </summary>
    [JsonPropertyName("MultWebProxy")]
    public List<ProxyData>? MultWebProxy { get; set; }
    /// <summary>
    /// ipinfo.io Api Token
    /// </summary>
    [JsonPropertyName("IpInfoToken")]
    public string? IpInfoToken { get; set; }
}
