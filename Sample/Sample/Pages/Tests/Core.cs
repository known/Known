using Known.AntBlazor.Components;

namespace Sample.Pages.Tests;

public class TestEntity : EntityBase
{
    public string Name { get; set; }

    [Category(nameof(ApplyType))]
    public string Type { get; set; }
}

public class TestInfo
{
    public string Text { get; set; }
    public int? Number { get; set; }
    public decimal? Decimal { get; set; }
    public DateTime? Date { get; set; } = DateTime.Now;
    public string Note { get; set; }
}

public class TestHelper
{
    public static List<CodeInfo> GetTexts()
    {
        var texts = new List<CodeInfo>();
        for (int i = 0; i < 50; i++)
        {
            texts.Add(new CodeInfo($"Code{i}", $"测试名称{i}"));
        }
        return texts;
    }
}

public class TestInfoForm : AntForm<TestInfo> { }