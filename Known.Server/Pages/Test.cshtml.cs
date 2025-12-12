namespace Known.Server.Pages;

public class TestModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    public string Id { get; set; }

    //Â·ÓÉ£ºÓòÃû/test?id=123242
    public async void OnGet(string id)
    {
        Id = id;
    }
}