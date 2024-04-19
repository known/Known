namespace Known.Blazor;

public class BaseForm : BaseComponent
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        await OnInitFormAsync();
    }

    protected virtual Task OnInitFormAsync() => Task.CompletedTask;

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildForm).Build();
    }

    protected virtual void BuildForm(RenderTreeBuilder builder) { }

    protected void OnToolClick(ActionInfo info) => OnAction(info, null);
    protected void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    protected bool HasButton(string buttonId)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        if (user.IsAdmin)
            return true;

        return IsInMenu(Id, buttonId);
    }

    private bool IsInMenu(string pageId, string buttonId)
    {
        var menu = Context.UserMenus.FirstOrDefault(m => m.Id == pageId || m.Code == pageId);
        if (menu == null)
            return false;

        var hasButton = false;
        if (menu.Tools != null && menu.Tools.Count > 0)
            hasButton = menu.Tools.Contains(buttonId);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(buttonId);
        return hasButton;
    }
}

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public bool ShowAction { get; set; }
    [Parameter] public FormModel<TItem> Model { get; set; }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        UI.BuildForm(builder, Model);
        if (ShowAction && !Model.IsView)
        {
            builder.FormAction(() =>
            {
                UI.Button(builder, new ActionInfo(Context, "OK", ""), this.Callback<MouseEventArgs>(OnSaveAsync));
                UI.Button(builder, new ActionInfo(Context, "Cancel", ""), this.Callback<MouseEventArgs>(OnCloseAsync));
            });
        }
    }

    protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);

    private async void OnSaveAsync(MouseEventArgs args) => await Model.SaveAsync();
    private async void OnCloseAsync(MouseEventArgs args) => await Model.CloseAsync();
}

public class BaseEditForm<TItem> : BaseForm<TItem> where TItem : class, new()
{
    private bool isEdit = false;

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        Model.IsView = !isEdit;
        BuildFormContent(builder);
    }

    protected virtual void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            UI.BuildForm(builder, Model);
            builder.FormPageButton(() => BuildAction(builder));
        });
    }

    protected void BuildAction(RenderTreeBuilder builder)
    {
        if (!isEdit)
        {
            UI.Button(builder, Language.Edit, this.Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
        }
        else
        {
            UI.Button(builder, Language.Save, this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
            UI.Button(builder, Language.Cancel, this.Callback<MouseEventArgs>(e => OnEdit(false)), "default");
        }
    }

    protected virtual Task<Result> OnSaveAsync(TItem model) => Result.SuccessAsync("");
    protected virtual void OnSuccess() { }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await OnSaveAsync(Model.Data);
        UI.Result(result, () =>
        {
            OnSuccess();
            OnEdit(false);
        });
    }

    private void OnEdit(bool edit) => isEdit = edit;
}

public class BaseTabForm : BaseForm
{
    protected TabModel Tab { get; } = new();

    protected override void BuildForm(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
}

public class BaseStepForm : BaseForm
{
    protected StepModel Step { get; } = new();

    protected override void BuildForm(RenderTreeBuilder builder) => UI.BuildSteps(builder, Step);
}