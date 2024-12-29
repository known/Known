using Known.Pages;
using Microsoft.AspNetCore.Components;

namespace Sample.WinForm.Pages;

[Route("/")]
[AntDesign.ReuseTabsPage(Title = "首页", Pin = true, Closable = false)]
public class Index : IndexPage { }