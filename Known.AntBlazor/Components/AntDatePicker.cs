using OneOf;

namespace Known.AntBlazor.Components;

public class AntDatePicker : DatePicker<DateTime?>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(DateTime?);
        base.OnInitialized();
    }
}

public class AntDateTimePicker : DatePicker<DateTime?>
{
    public AntDateTimePicker()
    {
        var format = "yyyy-MM-dd HH:mm";
        AutoFocus = true;
        ShowTime = true;
        Format = format;
        Mask = format;
        Placeholder = OneOf<string, string[]>.FromT0(format);
    }

    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(DateTime?);
        base.OnInitialized();
    }
}