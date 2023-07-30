namespace Known.Razor;

partial class UIService
{
    private bool isTop = false;

    internal DialogContainer TopDialog { get; set; }
    internal DialogContainer CurDialog { get; set; }

    internal void SetDialogMove(string dialogId) => InvokeVoidAsync("KRazor.setDialogMove", dialogId);

    public void Show(DialogOption option, bool isTop = false)
    {
        this.isTop = isTop;
        if (isTop)
            TopDialog?.Show(option);
        else
            CurDialog?.Show(option);
    }

    public void Show(IPicker picker)
    {
        var option = new DialogOption { IsMaxButton = true };
        if (picker != null)
        {
            option.Title = picker.Title;
            option.Size = picker.Size;
            option.Content = picker.BuildPick;
        }
        Show(option);
    }

    public void Show<T>(string title, Size? size, bool isMax = false, Action<AttributeBuilder<T>> action = null) where T : BaseComponent
    {
        Show(new DialogOption
        {
            Title = title,
            Size = size,
            IsMaxButton = true,
            IsMax = isMax,
            Content = builder => builder.Component(action)
        });
    }

    public void ShowForm<T>(string title, object model, Action<Result> onSuccess = null, Size? size = null, Action<AttributeBuilder<T>> action = null) where T : Form
    {
        var dialog = typeof(T).GetCustomAttribute<DialogAttribute>();
        if (size == null)
        {
            size = dialog != null
                 ? new Size(dialog.Width, dialog.Height)
                 : new Size(800, 600);
        }
        Show<T>(title, size, dialog != null && dialog.IsMax, attr =>
        {
            attr.Set(c => c.InDialog, true)
                .Set(c => c.ReadOnly, onSuccess == null)
                .Set(c => c.Model, model)
                .Set(c => c.OnSuccess, onSuccess);
            action?.Invoke(attr);
        });
    }

    public void CloseDialog()
    {
        if (isTop)
            TopDialog?.Close();
        else
            CurDialog?.Close();
        isTop = false;
    }
}