using System;
using System.Collections.Generic;
using System.Text;
using Known.Extensions;

namespace Known.Data
{
    public class Command
    {
        public Command(string text) : this(text, null)
        {
        }

        public Command(string text, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            Text = text;
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        public string Text { get; }
        public Dictionary<string, object> Parameters { get; }

        public bool HasParameter
        {
            get { return Parameters.Count > 0; }
        }

        public void AddParameter(string name, object value)
        {
            Parameters[name] = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Text={Text}");
            if (HasParameter)
            {
                var parameters = Parameters.ToJson();
                sb.AppendLine($"Parameters={parameters}");
            }
            return sb.ToString();
        }
    }
}
