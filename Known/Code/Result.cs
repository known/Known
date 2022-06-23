/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-15     KnownChen    移除ChartData类
 * ------------------------------------------------------------------------------- */

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

        public bool IsLoad { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; } = 10;
        public string Query { get; set; }
        public string[] OrderBys { get; set; }

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
            if (string.IsNullOrEmpty(Query))
                Query = "{}";

            var querys = Utils.FromJson<Dictionary<string, string>>(Query);
            if (querys != null && querys.Count > 0)
            {
                foreach (var item in querys)
                {
                    parameter[item.Key] = item.Value;
                }
            }
        }
    }
}