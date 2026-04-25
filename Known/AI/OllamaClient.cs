namespace Known.AI;

class OllamaClient
{
    private readonly ModelInfo model;
    private readonly HttpClient http;
    private readonly JsonSerializerOptions options;

    public OllamaClient(ModelInfo model)
    {
        this.model = model;
        http = new HttpClient { BaseAddress = new Uri(model.EndPoint) };
        options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    // 实现流式接口
    public async IAsyncEnumerable<string> GetChatCompletionsStreamAsync(List<ChatMessage> messages)
    {
        // 构建 Ollama 流式请求体
        var ollamaRequest = new
        {
            model = model.Model,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }),
            stream = true // 启用流式模式
        };

        // 发送请求并获取流式响应
        var response = await http.PostAsJsonAsync("api/chat", ollamaRequest, options);
        response.EnsureSuccessStatusCode();

        // 读取流式内容
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 解析 JSON 块
            var chunk = JsonSerializer.Deserialize<OllamaStreamChunk>(line, options);
            yield return chunk?.Message?.Content ?? "";

            if (chunk?.Done == true) break;
        }
    }

    private class OllamaStreamChunk
    {
        public OllamaMessage Message { get; set; }
        public bool Done { get; set; }
    }

    private class OllamaMessage
    {
        public string Role { get; set; }  // "user", "assistant"
        public string Content { get; set; }
    }
}