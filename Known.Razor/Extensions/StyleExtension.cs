namespace Known.Razor.Extensions;

public static class StyleExtension
{
    public static void AddRandomColor(this AttributeBuilder attr, string name)
    {
        if (!Setting.Info.RandomColor)
            return;

        var rndColor = Utils.GetRandomColor();
        var color = Utils.ToHtml(rndColor);
        attr.Style($"{name}:{color}");
    }
}