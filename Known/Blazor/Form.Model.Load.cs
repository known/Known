namespace Known.Blazor;

partial class FormModel<TItem>
{
    internal TItem DefaultData { get; set; }
    internal Func<Task<TItem>> DefaultDataAction { get; set; }

    /// <summary>
    /// 取得或设置表单关联的数据对象。
    /// </summary>
    public TItem Data { get; set; }

    /// <summary>
    /// 取得或设置表单加载时调用的委托。
    /// </summary>
    internal Func<TItem, Task> OnLoadData { get; set; }

    /// <summary>
    /// 异步加载表单数据。
    /// </summary>
    /// <param name="data">表单数据。</param>
    /// <returns></returns>
    public Task LoadDataAsync(TItem data) => OnLoadData?.Invoke(data);

    /// <summary>
    /// 异步加载表单默认数据。
    /// </summary>
    /// <returns></returns>
    public async Task LoadDefaultDataAsync()
    {
        if (!IsNew)
            return;

        var data = GetDefaultData();
        data ??= await DefaultDataAction?.Invoke();
        Data = data ?? new TItem();
    }

    internal void LoadDefaultData()
    {
        if (!IsNew)
            return;

        var data = GetDefaultData();
        Data = data ?? new TItem();
    }

    private TItem GetDefaultData()
    {
        if (DefaultData == null)
            return DefaultData;

        if (IsDictionary)
        {
            var data = new Dictionary<string, object>();
            var items = DefaultData as Dictionary<string, object>;
            foreach (var item in items)
            {
                data[item.Key] = item.Value;
            }
            return data as TItem;
        }
        else
        {
            var data = Activator.CreateInstance<TItem>();
            var baseProperties = TypeHelper.Properties(typeof(EntityBase));
            var properties = TypeHelper.Properties(typeof(TItem));
            foreach (var item in properties)
            {
                if (!item.CanWrite || baseProperties.Any(p => p.Name == item.Name))
                    continue;

                var value = item.GetValue(DefaultData);
                item.SetValue(data, value, null);
            }
            return data;
        }
    }
}