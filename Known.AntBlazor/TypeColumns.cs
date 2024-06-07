namespace Known.AntBlazor;

public class DictionaryColumn : PropertyColumn<Dictionary<string, object>, object> { }
public class StringColumn : Column<string> { }
public class BooleanColumn : Column<bool> { }
public class IntegerColumn : Column<int?> { }
public class DecimalColumn : Column<decimal?> { }

public class DateColumn : Column<DateTime?>
{
    public DateColumn()
    {
        Format = Config.DateFormat;
        Width = "120";
        Align = ColumnAlign.Center;
    }
}

public class DateTimeColumn : Column<DateTime?>
{
    public DateTimeColumn()
    {
        Format = Config.DateTimeFormat;
        Width = "150";
        Align = ColumnAlign.Center;
    }
}