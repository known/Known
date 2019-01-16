using System;
using System.Reflection;

namespace Known.Mapping
{
    public class PropertyMapper
    {
        internal PropertyMapper(PropertyInfo property, string[] keys = null)
        {
            Info = new ColumnInfo(property, keys);
        }

        public ColumnInfo Info { get; }

        public PropertyMapper IsStringColumn(string columnName, string description, bool required = false)
        {
            Info.Attribute = new StringColumnAttribute(columnName, description, required);
            return this;
        }

        public PropertyMapper IsStringColumn(string columnName, string description, int length, bool required = false)
        {
            Info.Attribute = new StringColumnAttribute(columnName, description, length, required);
            return this;
        }

        public PropertyMapper IsStringColumn(string columnName, string description, int minLength, int maxLength, bool required = false)
        {
            Info.Attribute = new StringColumnAttribute(columnName, description, minLength, maxLength, required);
            return this;
        }

        public PropertyMapper IsIntegerColumn(string columnName, string description, bool required = false)
        {
            Info.Attribute = new IntegerColumnAttribute(columnName, description, required);
            return this;
        }

        public PropertyMapper IsEnumColumn(string columnName, string description)
        {
            Info.Attribute = new EnumColumnAttribute(columnName, description);
            return this;
        }

        public PropertyMapper IsDecimalColumn(string columnName, string description, int integerLength, int decimalLength, bool required = false)
        {
            Info.Attribute = new DecimalColumnAttribute(columnName, description, integerLength, decimalLength, required);
            return this;
        }

        public PropertyMapper IsDateTimeColumn(string columnName, string description, bool required = false)
        {
            Info.Attribute = new DateTimeColumnAttribute(columnName, description, required);
            return this;
        }

        public PropertyMapper IsDateTimeColumn(string columnName, string description, string format, bool required = false)
        {
            Info.Attribute = new DateTimeColumnAttribute(columnName, description, format, required);
            return this;
        }

        public PropertyMapper IsDateTimeColumn(string columnName, string description, DateTime? minValue, DateTime? maxValue, bool required = false)
        {
            Info.Attribute = new DateTimeColumnAttribute(columnName, description, minValue, maxValue, required);
            return this;
        }

        public PropertyMapper IsBooleanColumn(string columnName, string description)
        {
            Info.Attribute = new BooleanColumnAttribute(columnName, description);
            return this;
        }
    }
}
