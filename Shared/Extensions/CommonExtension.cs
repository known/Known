namespace Known.Extensions;

/// <summary>
/// 通用扩展类。
/// </summary>
public static class CommonExtension
{
    #region String
    /// <summary>
    /// 追加一行格式化字符串。
    /// </summary>
    /// <param name="sb">字符串建造者。</param>
    /// <param name="format">格式字符串。</param>
    /// <param name="args">格式参数集合。</param>
    public static void AppendLine(this StringBuilder sb, string format, params object[] args)
    {
        var value = string.Format(format, args);
        sb.AppendLine(value);
    }
    #endregion

    #region Object
    /// <summary>
    /// 获取对象指定类型的属性值。
    /// </summary>
    /// <typeparam name="T">属性值类型。</typeparam>
    /// <param name="obj">对象。</param>
    /// <param name="propertyName">属性名。</param>
    /// <returns>属性值。</returns>
    public static T Property<T>(this object obj, string propertyName)
    {
        var value = obj.Property(propertyName);
        if (value == null)
            return default;

        return Utils.ConvertTo<T>(value);
    }

    /// <summary>
    /// 获取对象属性值。
    /// </summary>
    /// <param name="obj">对象。</param>
    /// <param name="propertyName">属性名。</param>
    /// <returns>属性值。</returns>
    public static object Property(this object obj, string propertyName)
    {
        if (obj == null)
            return null;

        if (!obj.GetType().IsDictionary())
            return TypeHelper.GetPropertyValue(obj, propertyName);

        (obj as Dictionary<string, object>).TryGetValue(propertyName, out object value);
        return value;
    }

    /// <summary>
    /// 将T2对象属性应用到T1对象上，T2继承T1。
    /// </summary>
    /// <typeparam name="T1">类型1。</typeparam>
    /// <typeparam name="T2">类型2。</typeparam>
    /// <param name="obj">对象1。</param>
    /// <param name="other">对象2。</param>
    public static void Apply<T1, T2>(this T1 obj, T2 other) where T2 : T1
    {
        if (obj == null || other == null)
            return;

        var type = typeof(T1);
        foreach (var property in TypeHelper.Properties(type))
        {
            if (property.CanRead && property.CanWrite)
            {
                var value = property.GetValue(other);
                if (value != null)
                    property.SetValue(obj, value);
            }
        }
    }

    /// <summary>
    /// 采用反射方式克隆对象的新实例。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="obj">原对象。</param>
    /// <returns>新对象。</returns>
    public static T Clone<T>(this T obj)
    {
        if (obj == null)
            return default;

        var type = obj.GetType();
        if (type.IsValueType || obj is string) // 如果是值类型或字符串，直接返回
            return obj;

        // 如果是 System.Reflection 类型，直接返回原对象
        if (type.Namespace != null && type.Namespace.StartsWith("System.Reflection"))
            return obj;

        // 如果是集合类型，递归克隆每个元素
        if (obj is IEnumerable enumerable)
        {
            var listType = typeof(List<>).MakeGenericType(type.GetGenericArguments());
            var list = (IList)Activator.CreateInstance(listType);
            foreach (var item in enumerable)
            {
                list.Add(item.Clone());
            }
            return (T)list;
        }

        // 创建新实例
        var clone = Activator.CreateInstance(type);
        foreach (var property in TypeHelper.Properties(type)) // 克隆属性
        {
            if (property.CanRead && property.CanWrite)
            {
                var value = property.GetValue(obj);
                if (value != null)
                    property.SetValue(clone, value.Clone());
            }
        }
        
        foreach (var field in TypeHelper.Fields(type)) // 克隆字段
        {
            var value = field.GetValue(obj);
            if (value != null)
                field.SetValue(clone, value.Clone());
        }

        return (T)clone;
    }

    /// <summary>
    /// 合并两个对象。
    /// </summary>
    /// <param name="obj1">对象1。</param>
    /// <param name="obj2">对象2。</param>
    /// <returns>合并后的对象。</returns>
    public static object Merge(this object obj1, object obj2)
    {
        if (obj1 == null) return null;
        if (obj2 == null) return obj1;

        var obj1Type = obj1.GetType();
        var obj2Type = obj2.GetType();
        var obj1Properties = obj1Type.GetProperties();
        var obj2Properties = obj2Type.GetProperties();

        var keyValues = new Dictionary<string, Type>();
        foreach (var prop in obj1Properties)
            keyValues[prop.Name] = prop.PropertyType;
        foreach (var prop in obj2Properties)
            keyValues[prop.Name] = prop.PropertyType;

        var mergedType = TypeHelper.CreateType(keyValues);
        var mergedObject = Activator.CreateInstance(mergedType);

        foreach (var property in obj1Properties)
        {
            var value = obj1Type.GetProperty(property.Name).GetValue(obj1, null);
            mergedType.GetProperty(property.Name).SetValue(mergedObject, value, null);
        }

        foreach (var property in obj2Properties)
        {
            var value = obj2Type.GetProperty(property.Name).GetValue(obj2, null);
            mergedType.GetProperty(property.Name).SetValue(mergedObject, value, null);
        }

        return mergedObject;
    }

    /// <summary>
    /// 合并两个泛型对象，返回动态对象。
    /// </summary>
    /// <typeparam name="TLeft">对象1类型。</typeparam>
    /// <typeparam name="TRight">对象2类型。</typeparam>
    /// <param name="left">对象1。</param>
    /// <param name="right">对象2。</param>
    /// <returns>合并后的对象。</returns>
    public static ExpandoObject Merge<TLeft, TRight>(this TLeft left, TRight right)
    {
        var expando = new ExpandoObject();
        IDictionary<string, object> dict = expando;
        foreach (var p in typeof(TLeft).GetProperties())
            dict[p.Name] = p.GetValue(left);
        foreach (var p in typeof(TRight).GetProperties())
            dict[p.Name] = p.GetValue(right);
        return expando;
    }
    #endregion

    #region List
    /// <summary>
    /// 获取列表分页结果。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="lists">数据列表。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    public static PagingResult<T> ToPagingResult<T>(this List<T> lists, PagingCriteria criteria)
    {
        if (lists == null || lists.Count == 0)
            return new PagingResult<T>(0, []);

        var pageData = lists.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
        var result = new PagingResult<T>(lists.Count, pageData);

        if (criteria.ExportMode != ExportMode.None)
            result.ExportData = DbUtils.GetExportData(criteria, pageData);

        return result;
    }
    #endregion
}