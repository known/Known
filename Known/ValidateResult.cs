using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known
{
    public class ValidateResult
    {
        public ValidateResult() : this(null) { }

        public ValidateResult(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                IsSuccess = true;
                AlertMessage = "保存成功！";
            }
            else
            {
                IsSuccess = false;
                AlertMessage = message;
            }
        }

        public bool IsSuccess { get; private set; }
        public string AlertMessage { get; private set; }
    }
}
