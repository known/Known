namespace Sample.Web.Pages;

public class TestModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    public string Id { get; set; }

    public void OnGet(string id)
    {
        Id = id;
    }
}