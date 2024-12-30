using Known.Plugins;
using Microsoft.AspNetCore.Components;

namespace Sample.WinForm.Pages;

[Route("/dev/test")]
[DevPlugin("测试", "file", Sort = 1)]
public class DevPage : BasePage
{
}