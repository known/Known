namespace Known;

public enum ColumnType { Text, Number, Boolean, Date, DateTime }
public enum AlignType { Left, Center, Right }

public class ColumnInfo
{
    public ColumnInfo()
    {
        Type = ColumnType.Text;
        Align = AlignType.Left;
    }

    public ColumnInfo(string name, string id, ColumnType type = ColumnType.Text, bool isQuery = false) : this()
    {
        Id = id;
        Name = name;
        Type = type;
        IsQuery = isQuery;
        SetColumnType();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public ColumnType Type { get; set; }
    public AlignType Align { get; set; }
    public int Width { get; set; }
    public int Sort { get; set; }
    public bool IsQuery { get; set; }
    public bool IsAdvQuery { get; set; }
    public bool IsSum { get; set; }
    public bool IsSort { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    public bool IsFixed { get; set; }

    public void SetColumnType(Type type)
    {
        var typeName = type.FullName;
        if (typeName.Contains("System.Boolean"))
            Type = ColumnType.Boolean;
        else if (typeName.Contains("System.DateTime"))
            Type = ColumnType.Date;
        else if (typeName.Contains("System.Decimal") || typeName.Contains("System.Int32"))
            Type = ColumnType.Number;

        SetColumnType();
    }

    private void SetColumnType()
    {
        if (Type == ColumnType.Boolean || Type == ColumnType.Date || Type == ColumnType.DateTime)
            Align = AlignType.Center;
        else if (Type == ColumnType.Number)
            Align = AlignType.Right;
    }
}