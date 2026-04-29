using System.Text;

namespace Known.Web.Services;

public static class SitemapService
{
    public static string BuildXml(DocumentService service)
    {
        var builder = new StringBuilder();
        builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        builder.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        foreach (var route in GetStaticUrls())
        {
            builder.AppendLine($"  <url><loc>https://known.org.cn{route}</loc></url>");
        }

        foreach (var route in service.GetSitemapUrls())
        {
            builder.AppendLine($"  <url><loc>https://known.org.cn{route}</loc></url>");
        }

        builder.AppendLine("</urlset>");
        return builder.ToString();
    }

    private static IReadOnlyList<string> GetStaticUrls()
    {
        return ["/", "/features", "/architecture", "/start", "/docs", "/ecosystem"];
    }
}