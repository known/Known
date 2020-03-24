using System;
using System.Reflection;

namespace Known.Mapping
{
    /// <summary>
    /// 属性映射器类。
    /// </summary>
    public class PropertyMapper
    {
        internal PropertyMapper(PropertyInfo property, string[] keys = null)
        {
            Info = new ColumnInfo(property, keys);
        }

        /// <summary>
        /// 取得栏位信息对象。
        /// </summary>
        public ColumnInfo Info { get; }

        /// <summary>
        /// 判断属性是否是字符型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsStringColumn(string columnName, string description, bool required = false)
        {
            Info.Attribute = new StringColumnAttribute(columnName, description, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是固定长度的字符型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="length">固定长度。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsStringColumn(string columnName, string description, int length, bool required = false)
        {
            Info.Attribute = new StringColumnAttribute(columnName, description, length, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是指定长度范围的字符型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="minLength">最小长度。</param>
        /// <param name="maxLength">最大长度。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsStringColumn(string columnName, string description, int minLength, int maxLength, bool required = false)
        {
            Info.Attribute = new StringColumnAttribute(columnName, description, minLength, maxLength, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是整型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsIntegerColumn(string columnName, string description, bool required = false)
        {
            Info.Attribute = new IntegerColumnAttribute(columnName, description, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是枚举型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsEnumColumn(string columnName, string description)
        {
            Info.Attribute = new EnumColumnAttribute(columnName, description);
            return this;
        }

        /// <summary>
        /// 判断属性是否是数值型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="integerLength">整数位长度。</param>
        /// <param name="decimalLength">小数位长度。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsDecimalColumn(string columnName, string description, int integerLength, int decimalLength, bool required = false)
        {
            Info.Attribute = new DecimalColumnAttribute(columnName, description, integerLength, decimalLength, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是日期时间型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsDateTimeColumn(string columnName, string description, bool required = false)
        {
            Info.Attribute = new DateTimeColumnAttribute(columnName, description, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是指定格式的日期时间型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="format">日期时间格式。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsDateTimeColumn(string columnName, string description, string format, bool required = false)
        {
            Info.Attribute = new DateTimeColumnAttribute(columnName, description, format, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是指定范围的日期时间型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="required">是否必填，默认 False。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsDateTimeColumn(string columnName, string description, DateTime? minValue, DateTime? maxValue, bool required = false)
        {
            Info.Attribute = new DateTimeColumnAttribute(columnName, description, minValue, maxValue, required);
            return this;
        }

        /// <summary>
        /// 判断属性是否是布尔型栏位。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <returns>属性映射器对象。</returns>
        public PropertyMapper IsBooleanColumn(string columnName, string description)
        {
            Info.Attribute = new BooleanColumnAttribute(columnName, description);
            return this;
        }
    }
}
