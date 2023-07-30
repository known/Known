﻿namespace Known.Razor.Pages.Forms;

class ColumnSetting : EditGrid<ColumnInfo>
{
    private List<ColumnInfo> orgData;

    [Parameter] public string PageId { get; set; }
    [Parameter] public Action OnSetting { get; set; }

    protected override void OnInitialized()
    {
        var builder = new ColumnBuilder<ColumnInfo>();
        builder.Field(r => r.Name).Name("名称").Edit().Width(100);
        builder.Field(r => r.Align).Name("对齐").Edit(new SelectOption(typeof(AlignType))).Width(100);
        builder.Field(r => r.Width).Name("宽度").Edit<Number>().Center(100);
        builder.Field(r => r.IsVisible).Name("显示").Edit();
        builder.Field(r => r.IsQuery).Name("查询").Edit();
        builder.Field(r => r.IsAdvQuery).Name("高级查询").Edit();
        builder.Field(r => r.IsSort).Name("排序").Edit();
        Columns = builder.ToColumns();

        Style = "form-grid";
        ActionHead = null;
        Actions = new List<ButtonInfo> { GridAction.MoveUp, GridAction.MoveDown };

        orgData = Data;
        Data = Setting.GetUserColumns(PageId, orgData);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Form(base.BuildRenderTree, BuildAction);
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        if (OnSetting != null)
            builder.Button(FormButton.Reset, Callback(OnReset));
        builder.Button(FormButton.OK, Callback(OnOK));
        builder.Button(FormButton.Cancel, Callback(OnCancel));
    }

    private async void OnReset()
    {
        var info = new SettingFormInfo { Type = UserSetting.KeyColumn, Name = PageId };
        await Platform.User.DeleteSettingAsync(info);
        Setting.UserSetting.Columns.Remove(PageId);
        Data = Setting.GetUserColumns(PageId, orgData);
        StateChanged();
        OnSetting?.Invoke();
    }

    private async void OnOK()
    {
        if (OnSetting != null)
        {
            var info = new SettingFormInfo
            {
                Type = UserSetting.KeyColumn,
                Name = PageId,
                Data = Utils.ToJson(Data)
            };
            await Platform.User.SaveSettingAsync(info);
            Setting.UserSetting.Columns[PageId] = Data;
            OnSetting.Invoke();
        }
        OnCancel();
    }

    private void OnCancel() => UI.CloseDialog();
}