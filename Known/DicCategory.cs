namespace Known;

public class DicCategory
{
    static DicCategory()
    {
        Categories = new List<DicCategory>();
    }

    private DicCategory(string name, bool hasChild = false)
    {
        Name = name;
        HasChild = hasChild;
    }

    public string Name { get; set; }
    public bool HasChild { get; set; }
    public static List<DicCategory> Categories { get; }

    public static void AddCategories<T>()
    {
        var fields = typeof(T).GetFields();
        foreach (var item in fields)
        {
            if (!item.IsLiteral)
                continue;

            var value = item.GetRawConstantValue()?.ToString();
            Categories.Add(new DicCategory(value));
        }
    }
}