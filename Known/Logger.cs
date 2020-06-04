using System;
using System.IO;
using log4net;
using log4net.Config;

namespace Known
{
    public sealed class Logger
    {
        private static readonly ILog log;

        static Logger()
        {
            var configFile = $@"{Environment.CurrentDirectory}\log4net.config";
            if (File.Exists(configFile))
            {
                XmlConfigurator.Configure(new FileInfo(configFile));
            }
            else
            {
                var type = typeof(Logger);
                var resourceName = type.Namespace + ".log4net.config";
                var stream = type.Assembly.GetManifestResourceStream(resourceName);
                XmlConfigurator.Configure(stream);
            }

            log = LogManager.GetLogger("App");
        }

        public static void Debug(object message)
        {
            log.Debug(message);
        }

        public static void Error(object message)
        {
            log.Error(message);
        }

        public static void Fatal(object message)
        {
            log.Fatal(message);
        }

        public static void Info(object message)
        {
            log.Info(message);
        }

        public static void Warn(object message)
        {
            log.Warn(message);
        }
    }
}
