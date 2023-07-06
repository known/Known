namespace Known.Studio.Models;

class FieldInfo
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Length { get; set; }
    public bool Required { get; set; }
    public bool IsQuery { get; set; }
}