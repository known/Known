using Known.Sample.Pages.Produce;

namespace Known.Sample.Extensions;

static class UIExtension
{
    internal static Task PrintWorkAsync(this JSService service, TbWork info, string printer)
    {
        return service.PrintAsync<WorkPrint>(c =>
        {
            c.Set(c => c.Work, info);
            c.Set(c => c.Printer, printer);
        });
    }

    internal static void SetPackInfo(this TbWork info)
    {
        info.PackInfo = [];
        if (info.PackFields != null && info.PackFields.Count > 0)
        {
            foreach (var item in info.PackFields)
            {
                info.PackInfo[item.Name] = item.GetValue(info);
            }
        }
    }

    internal static void SetPackForm(this DynamicFormModel model, List<PackFieldInfo> fields)
    {
        model.Rows.Clear();
        model.Columns.Clear();
        if (fields != null && fields.Count > 0)
        {
            foreach (var item in fields)
            {
                model.Columns[item.Name] = new ColumnInfo
                {
                    Id = item.Name,
                    Name = item.Name,
                    Type = GetFieldType(item),
                    ReadOnly = model.IsView || item.Work != WorkFieldType.None
                };
            }
        }
        model.InitFields();
    }

    private static FieldType GetFieldType(PackFieldInfo item)
    {
        if (item.Work != WorkFieldType.None)
            return FieldType.Text;

        return item.Type switch
        {
            AppFieldType.Text => FieldType.Text,
            AppFieldType.Integer => FieldType.Integer,
            AppFieldType.Decimal => FieldType.Number,
            AppFieldType.DateTime => FieldType.Date,
            _ => FieldType.Text,
        };
    }
}