namespace Known.Extensions;

static class TableExtension
{
    internal static void SetAdminTable<TItem>(this TableModel<TItem> model) where TItem : class, new()
    {
        model.EnableEdit = false;
        model.EnableFilter = UIConfig.IsAdvAdmin;
        model.AdvSearch = UIConfig.IsAdvAdmin;
    }

    internal static void SetDevTable<TItem>(this TableModel<TItem> model) where TItem : class, new()
    {
        model.EnableEdit = false;
        model.EnableFilter = false;
        model.AdvSearch = false;
    }
}