using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Known.Extensions
{
    public static class ObjectExtension
    {
        public static T To<T>(this object value)
        {
            if (value == null || value == DBNull.Value)
                return default(T);

            var valueString = value.ToString();
            var type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return default(T);

            if (type.IsEnum)
                return (T)Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            return (T)Convert.ChangeType(valueString, type);
        }

        public static object To(this object value, Type type)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var valueString = value.ToString();
            if (type == typeof(string))
                return Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return string.Empty;

            if (type.IsEnum)
                return Enum.Parse(type, value.ToString());

            if (type == typeof(bool) || type == typeof(bool?))
                return ",1,Y,YES,TRUE,".Contains(valueString.ToUpper());

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            return Convert.ChangeType(value, type);
        }

        public static bool In(this object value, params object[] array)
        {
            if (array == null || array.Length == 0)
                return false;

            return array.Contains(value);
        }

        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static string ToXml(this object value)
        {
            if (value == null)
                return null;

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

        public static byte[] Serialize(this object value)
        {
            if (value == null)
                return null;

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        public static object Deserialize(this byte[] buffer)
        {
            if (buffer == null)
                return null;

            using (var ms = new MemoryStream(buffer))
            {
                var bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }
    }
}
