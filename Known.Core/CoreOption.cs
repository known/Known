namespace Known.Core;

/// <summary>
/// 框架后端配置选项类。
/// </summary>
public class CoreOption
{
    /// <summary>
    /// 取得或设置响应数据是否启用压缩，默认禁用。
    /// </summary>
    public bool IsCompression { get; set; }

    /// <summary>
    /// 取得或设置是否动态生成WebApi，默认启用。
    /// </summary>
    public bool IsAddWebApi { get; set; } = true;
}