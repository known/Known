using System;
using System.Collections.Generic;
using System.Linq;

namespace Known.Validation
{
    public class ValidateResult
    {
        internal ValidateResult(List<ValidInfo> infos)
        {
            Infos = infos ?? new List<ValidInfo>();
        }

        public List<ValidInfo> Infos { get; }

        public bool HasError
        {
            get { return Infos.Exists(v => v.Level == ValidLevel.Error); }
        }

        public bool HasWarn
        {
            get { return Infos.Exists(v => v.Level == ValidLevel.Warn); }
        }

        public string ErrorMessage
        {
            get { return GetMessages(ValidLevel.Error); }
        }

        public string WarnMessage
        {
            get { return GetMessages(ValidLevel.Warn); }
        }

        private string GetMessages(ValidLevel level)
        {
            if (Infos == null || Infos.Count == 0)
                return string.Empty;

            var messages = Infos.Where(v => v.Level == level).Select(v => v.Message);
            return string.Join(Environment.NewLine, messages);
        }
    }
}