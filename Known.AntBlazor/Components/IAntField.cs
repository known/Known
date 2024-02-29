namespace Known.AntBlazor.Components;

public interface IAntField
{
    int Span { get; set; }
    string Label { get; set; }
    bool Required { get; set; }
}