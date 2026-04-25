namespace Known.AI;

/// <summary>
/// AI聊天模型类型枚举。
/// </summary>
public enum ChatType
{
    /// <summary>
    /// Ollama模型。
    /// </summary>
    [Description("Ollama模型")]
    Ollama = 1,
    /// <summary>
    /// 模拟输出。
    /// </summary>
    [Description("模拟输出")]
    Mock = 2,
    /// <summary>
    /// 扩展。
    /// </summary>
    [Description("扩展")]
    Extend = 3
}