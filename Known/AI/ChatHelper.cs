namespace Known.AI;

/// <summary>
/// 聊天助手类。
/// </summary>
public sealed class ChatHelper
{
    /// <summary>
    /// 分割文本为指定大小的块。
    /// </summary>
    /// <param name="text">要分割的文本。</param>
    /// <param name="size">每个块的大小。</param>
    /// <returns></returns>
    public static IEnumerable<string> SplitChunks(string text, int size)
    {
        if (string.IsNullOrWhiteSpace(text) || size <= 0)
            yield break;

        for (int i = 0; i < text.Length; i += size)
        {
            var len = Math.Min(size, text.Length - i);
            yield return text.Substring(i, len);
        }
    }
}