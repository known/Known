using System;
using System.IO;
using log4net;
using log4net.Config;

namespace Known
{
    public sealed class Logger
    {
        private static readonly ILog log;

        private Logger() { }

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
            Console.WriteLine($"DEBUD:{message}");
            log.Debug(message);
        }

        public static void Error(object message)
        {
            Console.WriteLine($"ERROR:{message}");
            log.Error(message);
        }

        public static void Fatal(object message)
        {
            Console.WriteLine($"FATAL:{message}");
            log.Fatal(message);
        }

        public static void Info(object message)
        {
            Console.WriteLine($"INFO:{message}");
            log.Info(message);
        }

        public static void Warn(object message)
        {
            Console.WriteLine($"WARN:{message}");
            log.Warn(message);
        }
    }
}
