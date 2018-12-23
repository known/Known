﻿namespace Known
{
    public class Result
    {
        protected Result(bool isValid, string message, object data = null)
        {
            IsValid = isValid;
            Message = message;
            Data = data;
        }

        public bool IsValid { get; }
        public string Message { get; }
        public object Data { get; }

        public static Result Error(string message, object data = null)
        {
            return new Result(false, message, data);
        }

        public static Result<T> Error<T>(string message, T data = default(T))
        {
            return new Result<T>(false, message, data);
        }

        public static Result Success(string message, object data = null)
        {
            return new Result(true, message, data);
        }

        public static Result<T> Success<T>(string message, T data = default(T))
        {
            return new Result<T>(true, message, data);
        }
    }

    public class Result<T> : Result
    {
        internal Result(bool isValid, string message, T data)
            : base(isValid, message)
        {
            Data = data;
        }

        public new T Data { get; }
    }
}
