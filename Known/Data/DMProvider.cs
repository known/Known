namespace Known.Data;

class DMProvider : DbProvider
{
    public override string Prefix => ":";

    public override string FormatName(string name) => $"\"{name}\"";
}