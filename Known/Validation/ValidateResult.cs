using System;
using System.Collections.Generic;
using System.Linq;

namespace Known.Validation
{
    public class ValidateResult
    {
        private List<ValidInfo> validInfos;

        public ValidateResult(List<ValidInfo> validInfos)
        {
            this.validInfos = validInfos ?? new List<ValidInfo>();
        }

        public bool HasError
        {
            get { return validInfos.Exists(v => v.Level == ValidLevel.Error); }
        }

        public bool HasWarn
        {
            get { return validInfos.Exists(v => v.Level == ValidLevel.Warn); }
        }

        public bool HasInfo
        {
            get { return validInfos.Exists(v => v.Level == ValidLevel.Info); }
        }

        public string ErrorMessage
        {
            get { return GetMessages(ValidLevel.Error); }
        }

        public string WarnMessage
        {
            get { return GetMessages(ValidLevel.Warn); }
        }

        public string InfoMessage
        {
            get { return GetMessages(ValidLevel.Info); }
        }

        private string GetMessages(ValidLevel level)
        {
            if (validInfos == null || validInfos.Count == 0)
                return string.Empty;

            var messages = validInfos.Where(v => v.Level == level);
            return string.Join(Environment.NewLine, messages);
        }
    }
}