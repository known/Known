﻿using Known.Designers;
using Known.Entities;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleForm : BaseForm<SysModule>
{
    private readonly StepModel step = new();
    private StepForm stepForm;

    private int StepCount
    {
        get
        {
            //TODO:数据语言切换
            if (Model.Data.Target != "页面")
                return 1;

            var page = Model.Data.Page;
            if (page != null && page.Columns.Exists(c => c.IsViewLink))
                return 5;

            return 4;
        }
    }

    internal EntityInfo Entity { get; set; }
    internal FlowInfo Flow { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        step.Items.Add(new(Context.Language.BasicInfo) { Content = BuildDataForm });
        step.Items.Add(new(Context.Language["Title.ModelSetting"]) { Content = BuildModuleModel });
        step.Items.Add(new(Context.Language["Title.FlowSetting"]) { Content = BuildModuleFlow });
        step.Items.Add(new(Context.Language["Title.PageSetting"]) { Content = BuildModulePage });
        step.Items.Add(new(Context.Language["Title.FormSetting"]) { Content = BuildModuleForm });
        Model.OnFieldChanged = OnFieldChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(this, b =>
        {
            b.Component<StepForm>()
             .Set(c => c.Model, step)
             .Set(c => c.IsView, Model.IsView)
             .Set(c => c.StepCount, StepCount)
             .Set(c => c.OnSave, SaveAsync)
             .Build(value => stepForm = value);
        });
    }

    private void BuildDataForm(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);

    private void BuildModuleModel(RenderTreeBuilder builder)
    {
        builder.Component<EntityDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Model, Model.Data.EntityData)
               .Set(c => c.OnChanged, model => Model.Data.EntityData = model)
               .Build();
    }

    private void BuildModuleFlow(RenderTreeBuilder builder)
    {
        builder.Component<FlowDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Entity, Entity)
               .Set(c => c.Model, Model.Data.FlowData)
               .Set(c => c.OnChanged, model => Model.Data.FlowData = model)
               .Build();
    }

    private void BuildModulePage(RenderTreeBuilder builder)
    {
        builder.Component<PageDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Entity, Entity)
               .Set(c => c.Model, Model.Data.Page)
               .Set(c => c.OnChanged, model =>
               {
                   Model.Data.Page = model;
                   stepForm.SetStepCount(StepCount);
               })
               .Build();
    }

    private void BuildModuleForm(RenderTreeBuilder builder)
    {
        builder.Component<FormDesigner>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Entity, Entity)
               .Set(c => c.Flow, Flow)
               .Set(c => c.Model, Model.Data.Form)
               .Set(c => c.OnChanged, model => Model.Data.Form = model)
               .Build();
    }

    private async Task<bool> SaveAsync(bool isClose = false)
    {
        if (!Model.Validate())
            return false;

        await Model.SaveAsync(isClose);
        await DataHelper.InitializeAsync(Platform.Module);
        return true;
    }

    private void OnFieldChanged(string columnId)
    {
        if (columnId == nameof(SysModule.Target))
        {
            stepForm.SetStepCount(StepCount);
        }
    }
}