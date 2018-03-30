namespace Known.Files
{
    public interface ISheetCell
    {
        int Row { get; }
        int Column { get; }
        string Name { get; }
        string StringValue { get; }
        string DisplayStringValue { get; }
        object Value { get; }
        void PutValue(object value);
    }
}
