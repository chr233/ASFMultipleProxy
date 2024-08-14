namespace ASFMultipleProxy.Data;

/// <summary>
/// 插件配置
/// </summary>
public sealed record PluginConfig
{
    /// <summary>
    /// 是否同意使用协议
    /// </summary>
    public bool EULA { get; set; } = true;
    /// <summary>
    /// 启用统计信息
    /// </summary>
    public bool Statistic { get; set; }
    /// <summary>
    /// 多个代理地址
    /// </summary>
    public List<ProxyData>? MultWebProxy { get; set; }
}
