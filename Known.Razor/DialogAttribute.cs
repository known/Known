namespace Known.Razor;

public class DialogAttribute : Attribute
{
    public DialogAttribute(int width, int height, bool isMax = false)
    {
        Width = width;
        Height = height;
        IsMax = isMax;
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsMax { get; set; }
}