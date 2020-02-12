using System;
using System.IO;
using System.Text;

namespace Known.Web
{
    class JavaScriptMinifier
    {
        #region 私有字段

        private const int EOF = -1;

        private readonly StringBuilder _JsBuilder;
        private readonly TextReader _JsReader;
        private int _TheA = Convert.ToInt32('\n');
        private int _TheB;
        private int _TheLookahead = EOF;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="JavaScriptMinifier"/> 类的新实例。
        /// </summary>
        /// <param name="jsReader">包含要压缩的 JavaScript 代码的 <see cref="TextReader"/>。</param>
        private JavaScriptMinifier(TextReader jsReader)
        {
            if (jsReader == null) 
                throw new ArgumentNullException("jsReader");

            this._JsReader = jsReader;
            this._JsBuilder = new StringBuilder();
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
            JavaScriptMinifier jsmin = new JavaScriptMinifier(jsReader);

            jsmin._Jsmin();

            return jsmin._JsBuilder;
        }

        #endregion

        #region 私有方法

        private void _Jsmin()
        {
            this._Action(3);

            while (this._TheA != EOF)
            {
                switch ((Char)this._TheA)
                {
                    case ' ':
                        if (_IsAlphanum(this._TheB)) this._Action(1);
                        else this._Action(2);

                        break;
                    case '\n':
                        switch ((Char)this._TheB)
                        {
                            case '{':
                            case '[':
                            case '(':
                            case '+':
                            case '-':
                                this._Action(1);

                                break;
                            case ' ':
                                this._Action(3);

                                break;
                            default:
                                if (_IsAlphanum(this._TheB)) this._Action(1);
                                else this._Action(2);

                                break;
                        }

                        break;
                    default:
                        switch ((Char)this._TheB)
                        {
                            case ' ':
                                if (_IsAlphanum(this._TheA))
                                {
                                    this._Action(1);

                                    break;
                                }

                                this._Action(3);

                                break;
                            case '\n':
                                switch ((Char)this._TheA)
                                {
                                    case '}':
                                    case ']':
                                    case ')':
                                    case '+':
                                    case '-':
                                    case '"':
                                    case '\'':
                                        this._Action(1);

                                        break;
                                    default:
                                        if (_IsAlphanum(this._TheA)) this._Action(1);
                                        else this._Action(3);

                                        break;
                                }

                                break;
                            default:
                                this._Action(1);

                                break;
                        }

                        break;
                }
            }
        }

        private void _Action(int d)
        {
            if (d <= 1) this._Put(this._TheA);
            if (d <= 2)
            {
                this._TheA = this._TheB;

                if (this._TheA == '\'' || this._TheA == '"')
                {
                    for (; ; )
                    {
                        this._Put(this._TheA);
                        this._TheA = this._Get();

                        if (this._TheA == this._TheB) break;
                        if (this._TheA <= '\n') throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}", this._TheA));
                        if (this._TheA != '\\') continue;

                        this._Put(this._TheA);
                        this._TheA = this._Get();
                    }
                }
            }

            if (d > 3) return;

            this._TheB = this._Next();

            if (this._TheB != '/' || ((((((((((((this._TheA != '(' && this._TheA != ',') && this._TheA != '=') && this._TheA != '[') && this._TheA != '!') && this._TheA != ':') && this._TheA != '&') && this._TheA != '|') && this._TheA != '?') && this._TheA != '{') && this._TheA != '}') && this._TheA != ';') && this._TheA != '\n')) return;

            this._Put(this._TheA);
            this._Put(this._TheB);

            for (; ; )
            {
                this._TheA = this._Get();

                if (this._TheA == '/') break;

                if (this._TheA == '\\')
                {
                    this._Put(this._TheA);
                    this._TheA = this._Get();
                }
                else if (this._TheA <= '\n') throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.", this._TheA));

                this._Put(this._TheA);
            }

            this._TheB = this._Next();
        }

        private int _Next()
        {
            int c = this._Get();
            const int s = (int)'*';

            if (c == '/')
            {
                switch ((Char)this._Peek())
                {
                    case '/':
                        for (; ; )
                        {
                            c = this._Get();

                            if (c <= '\n') return c;
                        }
                    case '*':
                        this._Get();

                        for (; ; )
                        {
                            switch (this._Get())
                            {
                                case s:
                                    if (this._Peek() == '/')
                                    {
                                        this._Get();

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

        private int _Peek()
        {
            this._TheLookahead = this._Get();

            return this._TheLookahead;
        }

        private int _Get()
        {
            int c = this._TheLookahead;
            this._TheLookahead = EOF;

            if (c == EOF) c = this._JsReader.Read();

            return c >= ' ' || c == '\n' || c == EOF ? c : (c == '\r' ? '\n' : ' ');
        }

        private void _Put(int c) { this._JsBuilder.Append((char)c); }

        private static bool _IsAlphanum(int c) { return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' || c > 126); }

        #endregion
    }
}
