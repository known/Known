namespace Sample.Wasm.Tests;

[Route("/test/table")]
public class TestTablePage : BaseTablePage<TestInfo>
{
}

public class TestInfo
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}