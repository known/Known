using System;
using System.Collections.Generic;

namespace Known
{
    public class Result
    {
        private readonly List<string> errors = new List<string>();
        private string message;

        public Result() { }

        private Result(string message, object data)
        {
            errors.Clear();
            IsValid = true;
            this.message = message;
            Data = data;
        }

        private bool ok;
        public bool Ok
        {
            get { return ok; }
            set { ok = IsValid = value; }
        }

        public bool IsValid { get; set; }

        public string Message
        {
            get
            {
                if (errors.Count == 0)
                    return message;

                return string.Join(Environment.NewLine, errors.ToArray());
            }
            set { message = value; }
        }

        public object Data { get; set; }

        public void AddError(string message)
        {
            IsValid = false;
            errors.Add(message);
        }

        public void Validate(bool broken, string message)
        {
            if (broken)
            {
                AddError(message);
            }
        }

        public static Result Error(string message, object data = null)
        {
            var result = new Result("", data);
            result.AddError(message);
            return result;
        }

        public static Result Success(string message, object data = null)
        {
            return new Result(message, data);
        }
    }

    public class PagingResult<T>
    {
        public int TotalCount { get; set; }
        public List<T> PageData { get; set; }
        public object Summary { get; set; }
    }

    public class PagingCriteria
    {
        private string compNo;
        private Dictionary<string, string> parameter;

        public PagingCriteria()
        {
            parameter = new Dictionary<string, string>();
        }

        public PagingCriteria(int pageIndex) : this()
        {
            PageIndex = pageIndex;
        }

        public PagingCriteria(string compNo) : this()
        {
            this.compNo = compNo;
        }

        public int load { get; set; }
        public int? page { get; set; }
        public int? limit { get; set; }
        public string field { get; set; }
        public string order { get; set; }
        public string query { get; set; }

        public bool IsLoad
        {
            get { return load == 1; }
        }

        public int? PageIndex
        {
            get { return page; }
            set { page = value; }
        }

        public int? PageSize
        {
            get { return limit ?? 10; }
            set { limit = value; }
        }

        public string[] OrderBys
        {
            get
            {
                var orderBys = new List<string>();
                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(order))
                {
                    var sorts = field.Split(',');
                    var orders = order.Split(',');
                    for (int i = 0; i < sorts.Length; i++)
                    {
                        orderBys.Add($"{sorts[i]} {orders[i]}");
                    }
                }
                return orderBys.ToArray();
            }
        }

        public Dictionary<string, string> Parameter
        {
            get
            {
                if (parameter.Count == 0)
                    InitParameter();

                if (!parameter.ContainsKey("CompNo"))
                {
                    if (string.IsNullOrEmpty(compNo))
                    {
                        var user = UserHelper.GetUser(out _);
                        compNo = user?.CompNo;
                    }
                    parameter["CompNo"] = compNo;
                }

                return parameter;
            }
            set { parameter = value; }
        }

        public bool HasParameter(string key)
        {
            if (Parameter == null)
                return false;

            if (Parameter.ContainsKey(key))
                return !string.IsNullOrEmpty(Parameter[key]);

            if (Parameter.ContainsKey($"L{key}"))
                return !string.IsNullOrEmpty(Parameter[$"L{key}"]);

            if (Parameter.ContainsKey($"G{key}"))
                return !string.IsNullOrEmpty(Parameter[$"G{key}"]);

            return false;
        }

        private void InitParameter()
        {
            if (string.IsNullOrEmpty(query))
                query = "{}";

            var querys = Utils.FromJson<Dictionary<string, string>>(query);
            if (querys != null && querys.Count > 0)
            {
                foreach (var item in querys)
                {
                    parameter[item.Key] = item.Value;
                }
            }
        }
    }

    public class ChartData
    {
        public string Name { get; set; }
        public Dictionary<string, object> Series { get; set; }
    }
}