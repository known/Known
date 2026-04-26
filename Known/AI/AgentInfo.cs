namespace Known.AI;

/// <summary>
/// AI智能体信息类。
/// </summary>
public class AgentInfo
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得模型头像。
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 取得或设置问候语。
    /// </summary>
    public string Greet { get; set; }

    /// <summary>
    /// 取得或设置提示词。
    /// </summary>
    public string Prompt { get; set; }

    /// <summary>
    /// 取得或设置是否启用文件附件功能。
    /// </summary>
    public bool EnableFile { get; set; }

    /// <summary>
    /// 取得或设置附件文件类型（MIME类型或文件扩展名），多个类型逗号分隔。
    /// </summary>
    public string FileAccept { get; set; } = ".txt,.md,.markdown,.json";

    /// <summary>
    /// 取得或设置附件文件大小限制，单位为字节。
    /// </summary>
    public long FileMaxSize { get; set; } = 2 * 1024 * 1024;

    /// <summary>
    /// 取得或设置智能体聊天模型信息。
    /// </summary>
    public ModelInfo Model { get; set; }
}