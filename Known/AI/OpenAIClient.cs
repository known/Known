using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace Known.AI;

class OpenAIClient
{
    private readonly ModelInfo model;
    private readonly HttpClient http = new() { Timeout = TimeSpan.FromMinutes(5) };

    public OpenAIClient(ModelInfo model)
    {
        this.model = model;
    }

    public async IAsyncEnumerable<string> GetChatStreamAsync(List<ChatMessage> messages)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.EndPoint))
            yield break;

        var payload = new JsonObject
        {
            ["model"] = model.Model,
            ["stream"] = true,
            ["messages"] = new JsonArray([.. messages.Select(m => new JsonObject
            {
                ["role"] = m.Role,
                ["content"] = m.Content
            })])
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, model.EndPoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        if (!string.IsNullOrWhiteSpace(model.ApiKey))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", model.ApiKey);
        request.Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
        response.EnsureSuccessStatusCode();

        if (!contentType.Contains("event-stream", StringComparison.OrdinalIgnoreCase))
        {
            var text = await response.Content.ReadAsStringAsync();
            var content = TryGetMessageContent(text);
            if (!string.IsNullOrWhiteSpace(content))
                yield return content;
            else if (!string.IsNullOrWhiteSpace(text))
                yield return text;
            yield break;
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        string line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                continue;

            var data = line[5..].Trim();
            if (data == "[DONE]")
                yield break;

            var chunk = TryGetStreamChunk(data);
            if (!string.IsNullOrWhiteSpace(chunk))
                yield return chunk;
        }
    }

    private static string TryGetStreamChunk(string json)
    {
        try
        {
            var root = JsonNode.Parse(json);
            return root?["choices"]?[0]?["delta"]?["content"]?.ToString()
                ?? root?["choices"]?[0]?["message"]?["content"]?.ToString()
                ?? root?["message"]?["content"]?.ToString()
                ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string TryGetMessageContent(string json)
    {
        try
        {
            var root = JsonNode.Parse(json);
            return root?["choices"]?[0]?["message"]?["content"]?.ToString()
                ?? root?["message"]?["content"]?.ToString()
                ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}