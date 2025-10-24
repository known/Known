namespace Known.Blazor;

partial class FormModel<TItem>
{
    private bool isInitColumns = false;

    /// <summary>
    /// 初始化表单布局。
    /// </summary>
    public void Initialize()
    {
        if (isInitColumns)
            return;

        isInitColumns = true;
        InitColumns();
    }

    // 初始化表单布局。
    // <param name="info">表单配置信息。</param>
    internal void Initialize(FormInfo info)
    {
        SetFormInfo(info);
    }

    // 初始化无代码表单栏位。
    internal void InitColumns()
    {
        if (Columns.Count == 0)
            return;

        Rows.Clear();
        var fields = Columns.Values.Where(c => c.IsVisible && c.Type != FieldType.Hidden);
        var rowNos = fields.Select(c => c.Row).Distinct().OrderBy(r => r).ToList();
        if (rowNos.Count == 1)
        {
            foreach (var item in fields)
            {
                AddRow().AddColumn(item);
            }
        }
        else
        {
            foreach (var rowNo in rowNos)
            {
                var infos = fields.Where(c => c.Row == rowNo).OrderBy(c => c.Column).ToArray();
                AddRow().AddColumn(infos);
            }
        }
    }

    /// <summary>
    /// 设置无代码表单信息。
    /// </summary>
    /// <param name="info"></param>
    public void SetFormInfo(FormInfo info)
    {
        if (info == null)
            return;

        Info = info;
        InitColumns(info);
        InitColumns();

        if (info.IsContinue)
        {
            Footer = b =>
            {
                if (IsNew)
                    b.Button(Language.SaveContinue, Page.Callback<MouseEventArgs>(async e => await SaveContinueAsync()));
                b.Button(Language.SaveClose, Page.Callback<MouseEventArgs>(async e => await SaveAsync()));
                b.Button(Language.Close, Page.Callback<MouseEventArgs>(async e => await CloseAsync()), "default");
            };
        }
    }

    private void InitColumns(FormInfo form)
    {
        Columns.Clear();
        if (typeof(TItem).IsDictionary())
        {
            if (form != null && form.Fields != null && form.Fields.Count > 0)
            {
                foreach (var item in form.Fields)
                {
                    Columns[item.Id] = new ColumnInfo(item);
                }
            }
        }
        else
        {
            var allColumns = TypeCache.Model<TItem>().GetColumns(false);
            foreach (var column in allColumns)
            {
                var info = form?.Fields?.FirstOrDefault(p => p.Id == column.Id);
                if (info != null)
                {
                    column.SetFormFieldInfo(info);
                    Columns[column.Id] = column;
                }
            }
        }
    }
}