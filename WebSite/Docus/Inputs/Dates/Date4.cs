﻿namespace WebSite.Docus.Inputs.Dates;

class Date4 : BaseComponent
{
    private Date? date;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Date>("周别", "Week").Value("2023-W30")
               .Set(f => f.DateType, DateType.Week)
               .Build(value => date = value);
    }

    private void OnVisibleChanged(bool value) => date?.SetVisible(value);
    private void OnEnabledChanged(bool value) => date?.SetEnabled(value);
    private void SetValue() => date?.SetValue("2023-W10");
    private string? GetValue() => date?.Value;
}