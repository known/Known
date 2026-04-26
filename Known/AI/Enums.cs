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
    Ollama,
    /// <summary>
    /// OpenAI模型。
    /// </summary>
    [Description("OpenAI模型")]
    OpenAI,
    /// <summary>
    /// 模拟模型。
    /// </summary>
    [Description("模拟模型")]
    Mock,
    /// <summary>
    /// 扩展模型。
    /// </summary>
    [Description("扩展模型")]
    Extend
}