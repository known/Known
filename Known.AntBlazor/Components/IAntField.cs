namespace Known.AntBlazor.Components;

public interface IAntField
{
    Type ValueType { get; }
    int Span { get; set; }
    string Label { get; set; }
    bool Required { get; set; }
}