using Microsoft.AspNetCore.Components;

namespace Known.Server.Pages;

public partial class Index
{
    private string UserName => CurrentUser?.Name ?? CurrentUser?.UserName ?? "Semi";

    private readonly List<TeamMemberInfo> TeamMembers =
    [
        new() { Initial = "H", Name = "兰超然", Email = "mrx@example.com", ColorClass = "yellow" },
        new() { Initial = "Z", Name = "谢天", Email = "jack@example.com", ColorClass = "blue" },
        new() { Initial = "Z", Name = "周伟", Email = "moto@example.com", ColorClass = "red" },
        new() { Initial = "L", Name = "李强", Email = "jason@example.com", ColorClass = "teal" }
    ];

    public override RenderFragment GetPageTitle()
    {
        return GetPageTitle("home", Language.Home);
    }

    private sealed class TeamMemberInfo
    {
        public string Initial { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ColorClass { get; set; }
    }
}