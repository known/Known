namespace Sample.Razor;

static class Extension
{
    internal static T Random<T>(this IEnumerable<T> lists)
    {
        if (lists == null || !lists.Any())
            return default;

        var rand = new Random();
        var index = rand.Next(0, lists.Count());
        return lists.ElementAt(index);
    }
}