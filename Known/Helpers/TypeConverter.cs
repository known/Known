namespace Known.Helpers;

class TypeConverter
{
    public static object ConvertTo(Type targetType, object value)
    {
        if (value == null)
            return GetDefaultValue(targetType);

        // 如果已经是目标类型，直接返回
        if (targetType.IsInstanceOfType(value))
            return value;

        // 处理可空类型
        if (Nullable.GetUnderlyingType(targetType) is Type underlyingType)
            return ConvertTo(underlyingType, value);

        // 处理枚举类型
        if (targetType.IsEnum)
            return ConvertToEnum(targetType, value);

        // 处理数组类型
        if (targetType.IsArray)
            return ConvertToArray(targetType, value);

        // 处理集合类型（List, IEnumerable等）
        if (targetType.IsGenericType && targetType.GetInterface("IEnumerable") != null)
            return ConvertToCollection(targetType, value);

        // 处理自定义类/结构体
        if (targetType.IsClass || targetType.IsValueType)
            return ConvertToComplexType(targetType, value);

        // 基本类型转换
        return ConvertSimpleType(targetType, value);
    }

    private static object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static object ConvertToEnum(Type enumType, object value)
    {
        if (value is string strValue)
            return Enum.Parse(enumType, strValue, true);

        try
        {
            return Enum.ToObject(enumType, Convert.ChangeType(value, Enum.GetUnderlyingType(enumType)));
        }
        catch
        {
            throw new InvalidCastException($"无法将值 '{value}' 转换为枚举类型 {enumType.Name}");
        }
    }

    private static object ConvertToArray(Type arrayType, object value)
    {
        Type elementType = arrayType.GetElementType();

        if (value is Array sourceArray)
        {
            Array result = Array.CreateInstance(elementType, sourceArray.Length);
            for (int i = 0; i < sourceArray.Length; i++)
            {
                result.SetValue(ConvertTo(elementType, sourceArray.GetValue(i)), i);
            }
            return result;
        }

        if (value is IEnumerable enumerable)
        {
            var list = new List<object>();
            foreach (var item in enumerable)
            {
                list.Add(item);
            }
            return ConvertToArray(arrayType, list.ToArray());
        }

        // 单个值转换为单元素数组
        Array singleArray = Array.CreateInstance(elementType, 1);
        singleArray.SetValue(ConvertTo(elementType, value), 0);
        return singleArray;
    }

    private static object ConvertToCollection(Type collectionType, object value)
    {
        // 获取集合的元素类型
        Type elementType = collectionType.GetGenericArguments()[0];

        // 创建目标集合实例
        var result = Activator.CreateInstance(collectionType);
        var addMethod = collectionType.GetMethod("Add");

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                addMethod.Invoke(result, [ConvertTo(elementType, item)]);
            }
        }
        else
        {
            // 单个值转换为单元素集合
            addMethod.Invoke(result, [ConvertTo(elementType, value)]);
        }

        return result;
    }

    private static object ConvertToComplexType(Type targetType, object value)
    {
        // 如果值是字典类型，尝试映射到对象属性
        if (value is IDictionary<string, object> dict)
        {
            var instance = Activator.CreateInstance(targetType);
            foreach (var prop in targetType.GetProperties())
            {
                if (dict.TryGetValue(prop.Name, out var propValue) && prop.CanWrite)
                    prop.SetValue(instance, ConvertTo(prop.PropertyType, propValue));
            }
            return instance;
        }

        // 尝试使用类型转换器
        var converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(value.GetType()))
            return converter.ConvertFrom(value);

        throw new InvalidCastException($"无法将类型 {value.GetType().Name} 转换为 {targetType.Name}");
    }

    private static object ConvertSimpleType(Type targetType, object value)
    {
        try
        {
            // 处理常见类型转换
            if (targetType == typeof(Guid))
            {
                if (value is string str) return Guid.Parse(str);
                if (value is byte[] bytes) return new Guid(bytes);
            }

            if (targetType == typeof(DateTimeOffset) && value is DateTime dt)
                return new DateTimeOffset(dt);

            // 使用Convert.ChangeType处理基本类型
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            // 尝试使用类型转换器作为后备方案
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(value);

            throw new InvalidCastException($"无法将值 '{value}' 转换为类型 {targetType.Name}");
        }
    }
}