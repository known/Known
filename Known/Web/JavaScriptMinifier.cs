using System;
using System.IO;
using System.Text;

namespace Known.Web
{
    /// <summary>
    /// JavaScript 代码压缩。
    /// </summary>
    public class JavaScriptMinifier
    {
        #region 私有字段

        private const int EOF = -1;

        private readonly StringBuilder jsBuilder;
        private readonly TextReader jsReader;
        private int theA = Convert.ToInt32('\n');
        private int theB;
        private int theLookahead = EOF;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="JavaScriptMinifier"/> 类的新实例。
        /// </summary>
        /// <param name="jsReader">包含要压缩的 JavaScript 代码的 <see cref="TextReader"/>。</param>
        private JavaScriptMinifier(TextReader jsReader)
        {
            this.jsReader = jsReader ?? throw new ArgumentNullException("jsReader");
            jsBuilder = new StringBuilder();
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 压缩指定的 JavaScript 代码。
        /// </summary>
        /// <param name="js">包含要压缩的 JavaScript 代码的 <see cref="StringBuilder"/>。</param>
        /// <returns>返回包含压缩后的 JavaScript 代码的 <see cref="StringBuilder"/>。</returns>
        public static StringBuilder Minify(StringBuilder js)
        {
            return Minify(new StringReader(js.ToString()));
        }

        /// <summary>
        /// 压缩指定的 JavaScript 代码。
        /// </summary>
        /// <param name="jsCode">要压缩的 JavaScript 代码。</param>
        /// <returns>返回包含压缩后的 JavaScript 代码的 <see cref="StringBuilder"/>。</returns>
        public static StringBuilder Minify(string jsCode)
        {
            return Minify(new StringReader(jsCode));
        }

        /// <summary>
        /// 压缩指定的 JavaScript 代码。
        /// </summary>
        /// <param name="jsReader">包含要压缩的 JavaScript 代码的 <see cref="TextReader"/>。</param>
        /// <returns>返回包含压缩后的 JavaScript 代码的 <see cref="StringBuilder"/>。</returns>
        public static StringBuilder Minify(TextReader jsReader)
        {
            var jsmin = new JavaScriptMinifier(jsReader);

            jsmin.Jsmin();

            return jsmin.jsBuilder;
        }

        #endregion

        #region 私有方法

        private void Jsmin()
        {
            Action(3);

            while (theA != EOF)
            {
                switch ((Char)this.theA)
                {
                    case ' ':
                        if (IsAlphaNum(this.theB)) this.Action(1);
                        else this.Action(2);

                        break;
                    case '\n':
                        switch ((Char)this.theB)
                        {
                            case '{':
                            case '[':
                            case '(':
                            case '+':
                            case '-':
                                this.Action(1);

                                break;
                            case ' ':
                                this.Action(3);

                                break;
                            default:
                                if (IsAlphaNum(this.theB)) this.Action(1);
                                else this.Action(2);

                                break;
                        }

                        break;
                    default:
                        switch ((Char)this.theB)
                        {
                            case ' ':
                                if (IsAlphaNum(this.theA))
                                {
                                    this.Action(1);

                                    break;
                                }

                                this.Action(3);

                                break;
                            case '\n':
                                switch ((Char)this.theA)
                                {
                                    case '}':
                                    case ']':
                                    case ')':
                                    case '+':
                                    case '-':
                                    case '"':
                                    case '\'':
                                        this.Action(1);

                                        break;
                                    default:
                                        if (IsAlphaNum(this.theA)) this.Action(1);
                                        else this.Action(3);

                                        break;
                                }

                                break;
                            default:
                                this.Action(1);

                                break;
                        }

                        break;
                }
            }
        }

        private void Action(int d)
        {
            if (d <= 1) this.Put(this.theA);
            if (d <= 2)
            {
                this.theA = this.theB;

                if (this.theA == '\'' || this.theA == '"')
                {
                    for (; ; )
                    {
                        this.Put(this.theA);
                        this.theA = this.Get();

                        if (this.theA == this.theB) break;
                        if (this.theA <= '\n') throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}", this.theA));
                        if (this.theA != '\\') continue;

                        this.Put(this.theA);
                        this.theA = this.Get();
                    }
                }
            }

            if (d > 3) return;

            this.theB = this.Next();

            if (this.theB != '/' || ((((((((((((this.theA != '(' && this.theA != ',') && this.theA != '=') && this.theA != '[') && this.theA != '!') && this.theA != ':') && this.theA != '&') && this.theA != '|') && this.theA != '?') && this.theA != '{') && this.theA != '}') && this.theA != ';') && this.theA != '\n')) return;

            this.Put(this.theA);
            this.Put(this.theB);

            for (; ; )
            {
                this.theA = this.Get();

                if (this.theA == '/') break;

                if (this.theA == '\\')
                {
                    this.Put(this.theA);
                    this.theA = this.Get();
                }
                else if (this.theA <= '\n') throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.", this.theA));

                this.Put(this.theA);
            }

            this.theB = this.Next();
        }

        private int Next()
        {
            int c = this.Get();
            const int s = (int)'*';

            if (c == '/')
            {
                switch ((Char)this.Peek())
                {
                    case '/':
                        for (; ; )
                        {
                            c = this.Get();

                            if (c <= '\n') return c;
                        }
                    case '*':
                        this.Get();

                        for (; ; )
                        {
                            switch (this.Get())
                            {
                                case s:
                                    if (this.Peek() == '/')
                                    {
                                        this.Get();

                                        return Convert.ToInt32(' ');
                                    }

                                    break;
                                case EOF:
                                    throw new Exception("Error: JSMIN Unterminated comment.");
                            }
                        }
                    default:
                        return c;
                }
            }

            return c;
        }

        private int Peek()
        {
            theLookahead = Get();

            return theLookahead;
        }

        private int Get()
        {
            int c = theLookahead;
            theLookahead = EOF;

            if (c == EOF) c = jsReader.Read();

            return c >= ' ' || c == '\n' || c == EOF ? c : (c == '\r' ? '\n' : ' ');
        }

        private void Put(int c)
        {
            jsBuilder.Append((char)c);
        }

        private static bool IsAlphaNum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' || c > 126);
        }

        #endregion
    }
}
