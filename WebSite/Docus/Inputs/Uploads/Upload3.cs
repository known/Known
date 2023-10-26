namespace WebSite.Docus.Inputs.Uploads;

class Upload3 : BaseComponent
{
    private KUpload? upload;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<KUpload>("附件：", "Upload").Build(value => upload = value);
    }

    private void OnVisibleChanged(bool value) => upload?.SetVisible(value);
    private void OnEnabledChanged(bool value) => upload?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => upload?.SetReadOnly(value);
    private void SetValue() => upload?.SetValue("孙膑");
    private string? GetValue() => upload?.Value;
}