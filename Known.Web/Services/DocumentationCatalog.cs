using System.Text;
using System.Text.RegularExpressions;
using Markdig;

namespace Known.Web.Services;

public sealed class DocumentationCatalog
{
    private static readonly Regex SummaryLinkPattern = new(@"^\s*-\s*\[(?<title>.+?)\]\((?<path>.+?\.md)\)", RegexOptions.Compiled);
    private static readonly Regex MarkdownLinkPattern = new(@"\[(?<text>[^\]]+)\]\((?<target>(?!https?://|mailto:|#)[^)]+?\.md)(?<anchor>#[^)]+)?\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex HeadingPattern = new(@"^(?<hashes>#{1,3})\s+(?<title>.+?)\s*$", RegexOptions.Compiled);

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
        return string.IsNullOrEmpty(normalized) ? "/docs" : $"/docs/{EncodeRoute(normalized)}";
    }

    public DocPage? GetPage(string? route)
    {
        if (orderedPages.Count == 0)
        {
            return null;
        }

        var normalized = NormalizeRoute(route);
        return string.IsNullOrEmpty(normalized)
            ? orderedPages[0]
            : pageMap.TryGetValue(normalized, out var page) ? page : null;
    }

    public IReadOnlyList<DocPage> GetHighlights(int count)
    {
        return orderedPages.Take(count).ToList();
    }

    public IReadOnlyList<DocPage> Search(string? keyword, int take = 18)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return orderedPages.Take(take).ToList();
        }

        var terms = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return orderedPages
            .Select(page => new { Page = page, Score = GetSearchScore(page, terms) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Page.Index)
            .Take(take)
            .Select(x => x.Page)
            .ToList();
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
            var headings = ExtractHeadings(markdown);
            var enhancedMarkdown = AddHeadingAnchors(RewriteLocalLinks(markdown, relativePath), headings);
            var html = Markdown.ToHtml(enhancedMarkdown, pipeline);
            var searchText = BuildSearchText(title, excerpt, headings, markdown);

            pages[relativePath] = new DocPage(
                title,
                route,
                relativePath,
                html,
                excerpt,
                "文档",
                headings,
                searchText,
                0);
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
                : new[] { new DocSection("文档", allPages.Values.Select((page, index) => page with { Index = index }).ToList()) };
        }

        var result = new List<DocSection>();
        var currentTitle = "文档导读";
        var currentPages = new List<DocPage>();
        var orderedIndex = 0;

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

            currentPages.Add(page with { SectionTitle = currentTitle, Index = orderedIndex++ });
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

    private static string AddHeadingAnchors(string markdown, IReadOnlyList<DocHeading> headings)
    {
        if (headings.Count == 0)
        {
            return markdown;
        }

        var headingQueue = new Queue<DocHeading>(headings.Where(x => x.Level <= 3));
        var builder = new StringBuilder();

        foreach (var rawLine in markdown.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            var match = HeadingPattern.Match(line.Trim());
            if (match.Success && headingQueue.Count > 0)
            {
                var heading = headingQueue.Dequeue();
                builder.AppendLine($"{match.Groups["hashes"].Value} {match.Groups["title"].Value} {{#{heading.Id}}}");
            }
            else
            {
                builder.AppendLine(line);
            }
        }

        return builder.ToString();
    }

    private static IReadOnlyList<DocHeading> ExtractHeadings(string markdown)
    {
        var result = new List<DocHeading>();
        var slugCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawLine in markdown.Split('\n'))
        {
            var line = rawLine.Trim();
            var match = HeadingPattern.Match(line);
            if (!match.Success)
            {
                continue;
            }

            var level = match.Groups["hashes"].Value.Length;
            if (level > 3)
            {
                continue;
            }

            var title = Regex.Replace(match.Groups["title"].Value, @"\s*\{#.*?\}$", string.Empty).Trim();
            var slug = Slugify(title);
            if (slugCount.TryGetValue(slug, out var count))
            {
                count++;
                slugCount[slug] = count;
                slug = $"{slug}-{count}";
            }
            else
            {
                slugCount[slug] = 0;
            }

            result.Add(new DocHeading(title, slug, level));
        }

        return result;
    }

    private static string BuildSearchText(string title, string excerpt, IReadOnlyList<DocHeading> headings, string markdown)
    {
        var body = Regex.Replace(markdown, "[`#>*_\\-\\[\\]()|]", " ");
        var headingText = string.Join(' ', headings.Select(x => x.Title));
        return $"{title} {excerpt} {headingText} {body}";
    }

    private static int GetSearchScore(DocPage page, string[] terms)
    {
        var score = 0;
        foreach (var term in terms)
        {
            if (page.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 12;
            }

            if (page.SectionTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 6;
            }

            if (page.Excerpt.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 4;
            }

            if (page.Headings.Any(x => x.Title.Contains(term, StringComparison.OrdinalIgnoreCase)))
            {
                score += 5;
            }

            if (page.SearchText.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 2;
            }
        }

        return score;
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

    private static string Slugify(string value)
    {
        var builder = new StringBuilder();
        var previousDash = false;

        foreach (var ch in value.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
                previousDash = false;
                continue;
            }

            if ((char.IsWhiteSpace(ch) || ch is '-' or '_' or '.') && !previousDash && builder.Length > 0)
            {
                builder.Append('-');
                previousDash = true;
                continue;
            }

            if (ch > 127)
            {
                builder.Append(ch);
                previousDash = false;
            }
        }

        var slug = builder.ToString().Trim('-');
        return string.IsNullOrEmpty(slug) ? Guid.NewGuid().ToString("N")[..8] : slug;
    }
}

public sealed record DocSection(string Title, IReadOnlyList<DocPage> Pages);

public sealed record DocPage(
    string Title,
    string Route,
    string RelativePath,
    string Html,
    string Excerpt,
    string SectionTitle,
    IReadOnlyList<DocHeading> Headings,
    string SearchText,
    int Index);

public sealed record DocHeading(string Title, string Id, int Level);
