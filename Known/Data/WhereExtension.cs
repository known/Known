namespace Known.Data;

public static class WhereExtension
{
    public static bool In<T>(this T field, T[] array) => true;
    public static bool NotIn<T>(this T field, T[] array) => true;
    public static bool Like(this string field, string like) => true;
    public static bool NotLike(this string field, string like) => true;
}