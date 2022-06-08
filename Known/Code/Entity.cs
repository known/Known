using System;
using System.Collections.Generic;
#if !NET35
using System.Dynamic;
#endif
using System.Globalization;
using System.Linq;
using System.Text;

namespace Known
{
    public class EntityBase
    {
        public EntityBase()
        {
            IsNew = true;
            Id = Utils.GetGuid();
            CreateBy = "temp";
            CreateTime = DateTime.Now;
            Version = 1;
            AppId = "temp";
            CompNo = "temp";
        }

        internal virtual bool IsNew { get; set; }
        internal Dictionary<string, object> Original { get; set; }

        [Column("ID", "", false, "1", "50")]
        public string Id { get; set; }

        [Column(Language.CreateBy, "", true, "1", "50")]
        public string CreateBy { get; set; }

        [Column(Language.CreateTime, "", true)]
        public DateTime CreateTime { get; set; }

        [Column(Language.ModifyBy, "", false, "1", "50")]
        public string ModifyBy { get; set; }

        [Column(Language.ModifyTime, "", false)]
        public DateTime? ModifyTime { get; set; }

        [Column(Language.Version, "", true)]
        public int Version { get; set; }

        [Column("Extension")]
        public string Extension { get; set; }

        [Column("AppId", "", true, "1", "50")]
        public string AppId { get; set; }

        [Column(Language.CompNo, "", true, "1", "50")]
        public string CompNo { get; set; }

        public bool CheckIsNew()
        {
            return IsNew;
        }

#if NET35
        public void FillModel(DynamicObject model)
        {
            var properties = GetType().GetProperties();
            foreach (var pi in model)
            {
                var name = pi.Key;
                if (name == "Id")
                    continue;

                var value = pi.Value;
                var property = properties.FirstOrDefault(p => p.Name == name);
                if (property != null)
                {
                    value = Utils.ConvertTo(property.PropertyType, value);
                    property.SetValue(this, value, null);
                }
            }
        }
#endif

#if NET472
        public void FillModel(dynamic model)
        {
            var properties = GetType().GetProperties();
            var pis = model.Properties();
            foreach (var pi in pis)
            {
                var name = (string)pi.Name;
                if (name == "Id")
                    continue;

                var value = (object)pi.Value.Value;
                var property = properties.FirstOrDefault(p => p.Name == name);
                if (property != null)
                {
                    value = Utils.ConvertTo(property.PropertyType, value);
                    property.SetValue(this, value);
                }
            }
        }
#endif

#if NET6_0
        public void FillModel(ExpandoObject model)
        {
            var properties = GetType().GetProperties();
            foreach (var pi in model)
            {
                var name = pi.Key;
                if (name == "Id")
                    continue;

                var value = pi.Value;
                var property = properties.FirstOrDefault(p => p.Name == name);
                if (property != null)
                {
                    value = Utils.ConvertTo(property.PropertyType, value);
                    property.SetValue(this, value);
                }
            }
        }
#endif

        public Result Validate()
        {
            var type = GetType();
            var properties = type.GetProperties();
            var dicError = new Dictionary<string, List<string>>();

            foreach (var pi in properties)
            {
                var attrs = pi.GetCustomAttributes(true);
                foreach (var item in attrs)
                {
                    if (item is ColumnAttribute attr)
                    {
                        var errors = new List<string>();
                        var value = pi.GetValue(this, null);
                        attr.Validate(value, pi.PropertyType, errors);
                        if (errors.Count > 0)
                            dicError.Add(pi.Name, errors);
                        break;
                    }
                }
            }

            if (dicError.Count > 0)
            {
                var result = Result.Error("", dicError);
                foreach (var item in dicError.Values)
                {
                    item.ForEach(m => result.AddError(m));
                }
                return result;
            }

            return Result.Success("");
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(
            string description,
            string columnName = null,
            bool required = false,
            string minLength = null,
            string maxLength = null,
            string dateFormat = null)
        {
            Description = description;
            ColumnName = columnName;
            Required = required;
            MinLength = minLength;
            MaxLength = maxLength;
            DateFormat = dateFormat;
        }

        public string Description { get; }
        public string ColumnName { get; }
        public bool Required { get; }
        public string MinLength { get; }
        public string MaxLength { get; }
        public string DateFormat { get; }

        public virtual void Validate(object value, Type type, List<string> errors)
        {
            var valueString = value == null ? "" : value.ToString().Trim();
            if (Required && string.IsNullOrEmpty(valueString))
            {
                errors.Add(Language.NotEmpty.Format(Description));
                return;
            }
            else if (!string.IsNullOrEmpty(valueString))
            {
                var length = GetByteLength(valueString);
                if (!string.IsNullOrEmpty(MinLength) && length < int.Parse(MinLength))
                    errors.Add(Language.MinLength.Format(Description, MinLength));
                if (!string.IsNullOrEmpty(MaxLength) && length > int.Parse(MaxLength))
                    errors.Add(Language.MaxLength.Format(Description, MaxLength));

                var typeName = type.FullName;
                if (typeName.Contains("System.Int32"))
                {
                    if (!int.TryParse(value.ToString(), out int i))
                        errors.Add(Language.MustInteger.Format(Description));
                    if (!string.IsNullOrEmpty(MinLength) && i < int.Parse(MinLength))
                        errors.Add(Language.MustMinLength.Format(Description, MinLength));
                    if (!string.IsNullOrEmpty(MaxLength) && i > int.Parse(MaxLength))
                        errors.Add(Language.MustMaxLength.Format(Description, MaxLength));
                }

                if (typeName.Contains("System.Decimal"))
                {
                    if (!decimal.TryParse(value.ToString(), out decimal d))
                        errors.Add(Language.MustNumber.Format(Description));
                    if (!string.IsNullOrEmpty(MinLength) && d < decimal.Parse(MinLength))
                        errors.Add(Language.MustMinLength.Format(Description, MinLength));
                    if (!string.IsNullOrEmpty(MaxLength) && d > decimal.Parse(MaxLength))
                        errors.Add(Language.MustMaxLength.Format(Description, MaxLength));
                }

                if (typeName.Contains("System.DateTime"))
                {
                    if (string.IsNullOrEmpty(DateFormat))
                    {
                        if (!DateTime.TryParse(value.ToString(), out _))
                            errors.Add(Language.MustDateTime.Format(Description));
                    }
                    else
                    {
                        if (!DateTime.TryParseExact(valueString, DateFormat, null, DateTimeStyles.None, out _))
                            errors.Add(Language.MustDateFormat.Format(Description, DateFormat));
                    }
                }
            }
        }

        private static int GetByteLength(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            return Encoding.Default.GetBytes(value).Length;
        }
    }
}