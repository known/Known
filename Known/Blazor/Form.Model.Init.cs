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

    /// <summary>
    /// 初始化表单布局。
    /// </summary>
    /// <param name="info">表单配置信息。</param>
    public void Initialize(FormInfo info)
    {
        SetFormInfo(info);
        InitColumns();
    }

    /// <summary>
    /// 初始化无代码表单栏位。
    /// </summary>
    public void InitColumns()
    {
        if (columns == null || columns.Count == 0)
            return;

        Rows.Clear();
        var fields = columns.Where(c => c.IsVisible && c.Type != FieldType.Hidden);
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
        columns = GetFormColumns(info);

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

    private static List<ColumnInfo> GetFormColumns(FormInfo form)
    {
        var columns = new List<ColumnInfo>();
        if (typeof(TItem).IsDictionary())
        {
            columns = form?.Fields?.Select(f => new ColumnInfo(f)).ToList();
        }
        else
        {
            var allColumns = TypeCache.Model(typeof(TItem)).GetColumns(false);
            foreach (var column in allColumns)
            {
                var info = form?.Fields?.FirstOrDefault(p => p.Id == column.Id);
                if (info != null)
                {
                    column.SetFormFieldInfo(info);
                    columns.Add(column);
                }
            }
        }
        return columns;
    }
}