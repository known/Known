using System.Text.RegularExpressions;

namespace Known.Web.Services;

static class HighlightHelper
{
    public static string Highlight(string code, string language)
    {
        var encoded = System.Net.WebUtility.HtmlEncode(code);

        if (language is "csharp" or "cs")
        {
            encoded = HighlightPattern(encoded, @"\b(Task|List|Dictionary|IReadOnlyList|IEnumerable|StringBuilder|Regex|MarkdownPipelineBuilder|Markdown)\b", "known-token type");
            encoded = HighlightPattern(encoded, @"\b(using|namespace|public|private|protected|internal|sealed|static|class|record|interface|enum|void|string|int|bool|return|new|if|else|foreach|for|while|var|async|await|null|true|false)\b", "known-token keyword");
        }
        else if (language is "xml" or "html" or "razor")
        {
            encoded = HighlightPattern(encoded, @"(&lt;/?[A-Za-z0-9_.:-]+)", "known-token tag");
            encoded = HighlightPattern(encoded, @"\s([A-Za-z_:][A-Za-z0-9_:\-.]*)(=)", "known-token attr", "$1$2");
        }
        else if (language is "js" or "javascript" or "ts" or "typescript")
        {
            encoded = HighlightPattern(encoded, @"\b(const|let|var|function|return|if|else|for|while|try|catch|new|class|import|export|await|async|true|false|null)\b", "known-token keyword");
        }
        else if (language is "bash" or "shell" or "powershell")
        {
            encoded = HighlightPattern(encoded, @"(^|\s)(dotnet|git|cd|npm|pnpm|ls|dir)(?=\s|$)", "known-token keyword");
        }

        encoded = HighlightPattern(encoded, "\"[^\"\\r\\n]*\"|'[^'\\r\\n]*'", "known-token string");
        encoded = HighlightPattern(encoded, @"\b\d+(\.\d+)?\b", "known-token number");
        encoded = HighlightPattern(encoded, @"//.*?$", "known-token comment", regexOptions: RegexOptions.Multiline);
        encoded = HighlightPattern(encoded, @"/\*[\s\S]*?\*/", "known-token comment");
        encoded = HighlightPattern(encoded, @"&lt;!--[\s\S]*?--&gt;", "known-token comment");

        return encoded;
    }

    public static string GetLanguageLabel(string language)
    {
        return language switch
        {
            "cs" => "C#",
            "csharp" => "C#",
            "razor" => "Razor",
            "html" => "HTML",
            "xml" => "XML",
            "js" => "JavaScript",
            "ts" => "TypeScript",
            "bash" => "Bash",
            "shell" => "Shell",
            "powershell" => "PowerShell",
            _ => string.IsNullOrWhiteSpace(language) ? "Text" : language.ToUpperInvariant()
        };
    }

    private static string HighlightPattern(string input, string pattern, string className, string replacementGroup = "$0", RegexOptions regexOptions = RegexOptions.None)
    {
        return Regex.Replace(
            input,
            @"(^|>)(?<text>[^<>]+)(?=<|$)",
            match =>
            {
                var prefix = match.Groups[1].Value;
                var text = match.Groups["text"].Value;
                var highlighted = Regex.Replace(text, pattern, $"<span class=\"{className}\">{replacementGroup}</span>", regexOptions | RegexOptions.IgnoreCase);
                return $"{prefix}{highlighted}";
            },
            RegexOptions.Multiline);
    }
}