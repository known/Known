namespace Sample.WinForm;

[Route("/")]
public class Index : Razor.Index
{
    public Index()
    {
        TopMenu = true;
    }
}