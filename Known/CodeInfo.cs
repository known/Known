namespace Known
{
    public class CodeInfo
    {
        public CodeInfo(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public CodeInfo(string category, string code, string name)
        {
            Category = category;
            Code = code;
            Name = name;
        }

        public string Category { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }

        public string ToString(string format)
        {
            if (string.IsNullOrEmpty(format))
                return Name;

            return string.Format(format, Code, Name);
        }
    }
}
