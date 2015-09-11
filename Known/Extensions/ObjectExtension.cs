using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public static class ObjectExtension
    {
        public static T To<T>(this object value)
        {
            if (value == null || value == DBNull.Value) return default(T);

            var valueString = value.ToString();
            var type = typeof(T);
            if (type == typeof(string)) return (T)Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0) return default(T);

            if (type.IsEnum) return (T)Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            return (T)Convert.ChangeType(valueString, type);
        }

        public static object To(this object value, Type type)
        {
            if (value == null || value == DBNull.Value) return null;

            var valueString = value.ToString();
            if (type == typeof(string)) return Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0) return string.Empty;

            if (type.IsEnum) return Enum.Parse(type, value.ToString());

            if (type == typeof(bool) || type == typeof(bool?))
                return ",1,Y,YES,TRUE,".Contains(valueString.ToUpper());

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);//new NullableConverter(type).UnderlyingType;

            return Convert.ChangeType(value, type);
        }

        public static string ToJson(this object value)
        {
            if (value == null) return null;

            var serializer = new JavaScriptSerializer();
            if (value is DataTable)
            {
                var data = value as DataTable;
                if (data == null || data.Rows.Count == 0)
                    return null;

                var list = data.ToDictionaryList();
                return serializer.Serialize(list);
            }
            return serializer.Serialize(value);
        }

        public static string ToXml(this object value)
        {
            if (value == null) return null;

            var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
            using (var stream = new MemoryStream())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, namespaces);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
