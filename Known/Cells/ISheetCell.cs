namespace Known.Cells
{
    public interface ISheetCell
    {
        int Row { get; }
        int Column { get; }
        string Name { get; }
        string StringValue { get; }
        string DisplayStringValue { get; }
        string Formula { get; set; }
        object Value { get; set; }
    }
}
