using System;
using System.Collections.Generic;

namespace Known.Validation
{
    public class ValidInfo
    {
        public ValidInfo(ValidLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public ValidInfo(ValidLevel level, string field, List<string> messages)
        {
            Level = level;
            Field = field;
            Message = string.Join(Environment.NewLine, messages);
        }

        public ValidLevel Level { get; }
        public string Field { get; }
        public string Message { get; }
    }
}
