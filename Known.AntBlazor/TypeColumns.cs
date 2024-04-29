namespace Known.AntBlazor;

public class StringColumn : Column<string> { }
public class BooleanColumn : Column<bool> { }
public class IntegerColumn : Column<int?> { }
public class DecimalColumn : Column<decimal?> { }

public class DateColumn : Column<DateTime?>
{
    public DateColumn()
    {
        Format = "yyyy-MM-dd";
        Width = "120";
        Align = ColumnAlign.Center;
    }
}

public class DateTimeColumn : Column<DateTime?>
{
    public DateTimeColumn()
    {
        Format = "yyyy-MM-dd HH:ss";
        Width = "150";
        Align = ColumnAlign.Center;
    }
}

public class DictionaryColumn : PropertyColumn<Dictionary<string, object>, object> { }