namespace Known.Data;

public static class WhereExtension
{
    public static bool In<T>(this T field, T[] array) => true;
    public static bool NotIn<T>(this T field, T[] array) => true;
    public static bool Between<T>(this T field, T begin, T end) => true;
}