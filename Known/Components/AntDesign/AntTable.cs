using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant表格组件类。
/// </summary>
/// <typeparam name="TItem">表格数据对象类型。</typeparam>
public class AntTable<TItem> : Table<TItem>, IComContainer where TItem : class, new()
{
    private TableModel<TItem> _prevModel;
    private object _prevDataSource;
    private int _prevPageIndex;

    [Inject] private IServiceScopeFactory Factory { get; set; }

    /// <summary>
    /// 取得是否是表格。
    /// </summary>
    public bool IsTable => true;

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置表格组件模型对象实例。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <summary>
    /// 取得或设置是否只读。
    /// </summary>
    [Parameter] public bool IsView { get; set; }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns></returns>
    public Task<T> CreateServiceAsync<T>() where T : IService
    {
        var context = Model?.Context ?? Context;
        return Factory.CreateAsync<T>(context);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Size = TableSize.Small;
        Responsive = true;
        ScrollBarWidth = "8px";
        if (Model != null)
        {
            PageSizeChanged = this.Callback<int>(e => PageIndex = 1);
            if (Model.TreeChildren != null)
                TreeChildren = Model.TreeChildren;
            if (Model.RowClass != null)
                RowClassName = r => Model.RowClass.Invoke(r.Data);
        }
        PaginationPosition = "bottomRight";
        PaginationTemplate = this.BuildTree<(int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange)>(BuildPagination);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // 检查关键参数是否真的变化
        bool modelChanged = _prevModel != Model;
        bool dataSourceChanged = !ReferenceEquals(_prevDataSource, DataSource);
        bool pageIndexChanged = _prevPageIndex != PageIndex;

        if (modelChanged || dataSourceChanged || pageIndexChanged)
        {
            // 更新缓存
            _prevModel = Model;
            _prevDataSource = DataSource;
            _prevPageIndex = PageIndex;

            // 执行参数变化时的逻辑
            if (Model != null)
            {
                //Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {typeof(TItem).Name}：OnParametersSet");
                //Console.WriteLine(Environment.StackTrace);
                Class = CssBuilder.Default(Model.Class).AddClass("kui-striped", Model.IsStriped).BuildClass();
                Resizable = Model.Resizable;
                Bordered = Model.Bordered;
                AutoHeight = Model.AutoHeight;
                RowKey = Model.RowKey;
                PageIndex = Model.Criteria.PageIndex;
                PageSize = Model.Criteria.PageSize;
                HidePagination = !Model.ShowPager;
                DataSource = Model.DataSource;
                // 只执行一次初始化，避免重复调用
                if (modelChanged)
                    Model.OnInitial?.Invoke(this);
            }
        }
        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading<IComContainer>(this, base.BuildRenderTree);
    }

    private void BuildPagination(RenderTreeBuilder builder, (int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange) tuple)
    {
        if (Model != null && Model.BottomLeft != null)
        {
            builder.Div("kui-flex-space", () =>
            {
                builder.Fragment(Model.BottomLeft);
                BuildPager(builder, tuple);
            });
        }
        else
        {
            BuildPager(builder, tuple);
        }
    }

    private void BuildPager(RenderTreeBuilder builder, (int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange) tuple)
    {
        Func<PaginationTotalContext, string> showTotal = ctx => Context?.Language[Language.PageTotal].Replace("{total}", $"{ctx.Total}");
        builder.Component<Pagination>()
               .Set(c => c.Class, tuple.PaginationClass)
               .Set(c => c.Total, tuple.Total)
               .Set(c => c.PageSize, tuple.PageSize)
               .Set(c => c.Current, tuple.PageIndex)
               .Set(c => c.ShowTotal, showTotal)
               .Set(c => c.ShowSizeChanger, true)
               .Set(c => c.ShowQuickJumper, false)
               .Set(c => c.OnChange, tuple.HandlePageChange)
               .Build();
    }
}