using Microsoft.AspNetCore.Components;
using WebSite.Core;

namespace WebSite.Data;

class DocService
{
    internal static List<MenuItem> GetDocMenus()
    {
        return new List<MenuItem>
        {
            new MenuItem("overview", "概述")
            {
                Children = new List<MenuItem>
                {
                    new MenuItem("profile", "简介"),
                    new MenuItem("start", "快速开始"),
                }
            }
        };
    }

    internal static MarkupString GetDocHtml(string? id)
    {
        if (id == "profile")
        {
            var markdown = new Markdown();
            var text = File.ReadAllText(@"D:\Publics\Known\README.md");
            var html = markdown.Transform(text);
            return new MarkupString(html);
        }

        return new MarkupString(id);
    }
}