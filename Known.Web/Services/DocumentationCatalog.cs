using System.Text.RegularExpressions;
using Markdig;

namespace Known.Web.Services;

public sealed class DocumentationCatalog
{
    private static readonly Regex SummaryLinkPattern = new(@"^\s*-\s*\[(?<title>.+?)\]\((?<path>.+?\.md)\)", RegexOptions.Compiled);
    private static readonly Regex MarkdownLinkPattern = new(@"\[(?<text>[^\]]+)\]\((?<target>(?!https?://|mailto:|#)[^)]+?\.md)(?<anchor>#[^)]+)?\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly string docsRoot;
    private readonly IReadOnlyList<DocSection> sections;
    private readonly IReadOnlyList<DocPage> orderedPages;
    private readonly Dictionary<string, DocPage> pageMap;

    public DocumentationCatalog(IWebHostEnvironment environment)
    {
        docsRoot = Path.Combine(environment.ContentRootPath, "docs");

        if (!Directory.Exists(docsRoot))
        {
            sections = Array.Empty<DocSection>();
            orderedPages = Array.Empty<DocPage>();
            pageMap = new(StringComparer.OrdinalIgnoreCase);
            return;
        }

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var allPages = LoadPages(pipeline);
        sections = LoadSections(allPages);
        orderedPages = sections.SelectMany(x => x.Pages).ToList();
        pageMap = orderedPages
            .GroupBy(x => NormalizeRoute(x.Route), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<DocSection> Sections => sections;
    public int DocumentCount => orderedPages.Count;

    public string GetUrl(string? route)
    {
        var normalized = NormalizeRoute(route);
        if (string.IsNullOrEmpty(normalized))
        {
            return "/docs";
        }

        return $"/docs/{EncodeRoute(normalized)}";
    }

    public DocPage? GetPage(string? route)
    {
        if (orderedPages.Count == 0)
        {
            return null;
        }

        var normalized = NormalizeRoute(route);
        if (string.IsNullOrEmpty(normalized))
        {
            return orderedPages[0];
        }

        return pageMap.TryGetValue(normalized, out var page) ? page : null;
    }

    public IReadOnlyList<DocPage> GetHighlights(int count)
    {
        return orderedPages.Take(count).ToList();
    }

    public DocPage? GetPrevious(string route)
    {
        var index = FindIndex(route);
        return index > 0 ? orderedPages[index - 1] : null;
    }

    public DocPage? GetNext(string route)
    {
        var index = FindIndex(route);
        return index >= 0 && index < orderedPages.Count - 1 ? orderedPages[index + 1] : null;
    }

    private Dictionary<string, DocPage> LoadPages(MarkdownPipeline pipeline)
    {
        var pages = new Dictionary<string, DocPage>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.GetFiles(docsRoot, "*.md", SearchOption.AllDirectories))
        {
            var relativePath = NormalizeRelativePath(Path.GetRelativePath(docsRoot, file));
            if (relativePath.StartsWith("examples/", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var markdown = File.ReadAllText(file);
            var route = relativePath[..^3];
            var title = ExtractTitle(markdown, Path.GetFileNameWithoutExtension(file));
            var excerpt = ExtractExcerpt(markdown, title);
            var rewritten = RewriteLocalLinks(markdown, relativePath);
            var html = Markdown.ToHtml(rewritten, pipeline);

            pages[relativePath] = new DocPage(title, route, relativePath, html, excerpt, "文档");
        }

        return pages;
    }

    private IReadOnlyList<DocSection> LoadSections(Dictionary<string, DocPage> allPages)
    {
        var summaryPath = Path.Combine(docsRoot, "SUMMARY.md");
        if (!File.Exists(summaryPath))
        {
            return allPages.Count == 0
                ? Array.Empty<DocSection>()
                : new[] { new DocSection("文档", allPages.Values.ToList()) };
        }

        var result = new List<DocSection>();
        var currentTitle = "文档导读";
        var currentPages = new List<DocPage>();

        foreach (var rawLine in File.ReadLines(summaryPath))
        {
            var line = rawLine.Trim();
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                AddSection(result, currentTitle, currentPages);
                currentTitle = line[3..].Trim();
                currentPages = new List<DocPage>();
                continue;
            }

            var match = SummaryLinkPattern.Match(line);
            if (!match.Success)
            {
                continue;
            }

            var relativePath = NormalizeRelativePath(Uri.UnescapeDataString(match.Groups["path"].Value));
            if (!allPages.TryGetValue(relativePath, out var page))
            {
                continue;
            }

            currentPages.Add(page with { SectionTitle = currentTitle });
        }

        AddSection(result, currentTitle, currentPages);
        return result;
    }

    private string RewriteLocalLinks(string markdown, string currentRelativePath)
    {
        var currentDirectory = Path.GetDirectoryName(currentRelativePath)?.Replace('\\', '/') ?? string.Empty;
        return MarkdownLinkPattern.Replace(markdown, match =>
        {
            var target = Uri.UnescapeDataString(match.Groups["target"].Value);
            var anchor = match.Groups["anchor"].Value;
            var combined = string.IsNullOrEmpty(currentDirectory)
                ? target
                : NormalizeRelativePath(Path.Combine(currentDirectory, target));

            if (!combined.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                return match.Value;
            }

            var fullPath = Path.GetFullPath(Path.Combine(docsRoot, combined));
            if (!fullPath.StartsWith(docsRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
            {
                return match.Value;
            }

            var route = NormalizeRelativePath(Path.GetRelativePath(docsRoot, fullPath))[..^3];
            return $"[{match.Groups["text"].Value}]({GetUrl(route)}{anchor})";
        });
    }

    private static void AddSection(List<DocSection> sections, string title, List<DocPage> pages)
    {
        if (pages.Count > 0)
        {
            sections.Add(new DocSection(title, pages));
        }
    }

    private int FindIndex(string route)
    {
        var normalized = NormalizeRoute(route);
        for (var i = 0; i < orderedPages.Count; i++)
        {
            if (string.Equals(orderedPages[i].Route, normalized, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static string ExtractTitle(string markdown, string fallback)
    {
        foreach (var rawLine in markdown.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                return line[2..].Trim();
            }
        }

        return fallback;
    }

    private static string ExtractExcerpt(string markdown, string title)
    {
        foreach (var rawLine in markdown.Split('\n'))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) ||
                line.StartsWith("#", StringComparison.Ordinal) ||
                line.StartsWith("```", StringComparison.Ordinal) ||
                line.StartsWith("---", StringComparison.Ordinal) ||
                line.StartsWith("|", StringComparison.Ordinal) ||
                Regex.IsMatch(line, "^\\d+\\.", RegexOptions.CultureInvariant) ||
                line.StartsWith("- ", StringComparison.Ordinal))
            {
                continue;
            }

            return line.Length > 120 ? $"{line[..120]}..." : line;
        }

        return $"阅读 {title}，了解 Known Framework 的相关能力。";
    }

    private static string NormalizeRoute(string? route)
    {
        return NormalizeRelativePath(Uri.UnescapeDataString(route ?? string.Empty).Trim('/'));
    }

    private static string NormalizeRelativePath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static string EncodeRoute(string route)
    {
        return string.Join('/', NormalizeRelativePath(route)
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(Uri.EscapeDataString));
    }
}

public sealed record DocSection(string Title, IReadOnlyList<DocPage> Pages);

public sealed record DocPage(string Title, string Route, string RelativePath, string Html, string Excerpt, string SectionTitle);
