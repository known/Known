using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Commons.Collections;

namespace Known.Web
{
    public class TemplateEngine
    {
        private VelocityEngine velocity = null;
        private IContext context = null;

        public TemplateEngine(string templatePath)
        {
            velocity = new VelocityEngine();

            //使用设置初始化VelocityEngine
            var props = new ExtendedProperties();
            props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatePath);
            props.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            velocity.Init(props);
            //为模板变量赋值
            context = new VelocityContext();
        }

        public void Put(string key, object value)
        {
            context.Put(key, value);
        }

        public string BuildString(string templateFile)
        {
            //从文件中读取模板
            var template = velocity.GetTemplate(templateFile);
            //合并模板
            using (var writer = new StringWriter())
            {
                template.Merge(context, writer);
                return writer.ToString();
            }
        }
    }
}