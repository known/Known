﻿using AntDesign;

namespace Known.Components;

/// <summary>
/// 工具条组件类。
/// </summary>
public partial class KToolbar
{
    /// <summary>
    /// 取得或设置工具条模型。
    /// </summary>
    [Parameter] public ToolbarModel Model { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Model == null)
            return;

        Model.OnRefresh = StateChanged;
        base.OnInitialized();
    }

    private void OnEditTool()
    {
        Plugin?.EditToolbar(this);
    }

    private DropdownModel GetDropdownModel(string name)
    {
        return GetDropdownModel(new ActionInfo { Name = name });
    }

    private DropdownModel GetDropdownModel(ActionInfo item)
    {
        return new DropdownModel
        {
            ChildContent = b =>
            {
                b.Component<Button>()
                 .Set(c => c.Type, item.ToType())
                 .Set(c => c.ChildContent, b =>
                 {
                     b.Div("kui-toolbar-more", () =>
                     {
                         b.IconName(item.Icon, item.Name);
                         b.Icon("down");
                     });
                 })
                 .Build();
            },
            OnItemClick = e=>
            {
                Model.OnItemClick?.Invoke(e);
                return Task.CompletedTask;
            }
        };
    }
}