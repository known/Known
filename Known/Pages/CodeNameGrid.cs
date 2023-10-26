namespace Known.Pages;

class CodeNameGrid : KEditGrid<CodeName>
{
    public CodeNameGrid()
    {
        var builder = new ColumnBuilder<CodeName>();
        builder.Field("代码", nameof(CodeName.Code)).Width(60).Edit();
        builder.Field("名称", nameof(CodeName.Name)).Edit();
        Columns = builder.ToColumns();
    }
}