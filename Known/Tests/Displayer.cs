using System;

namespace Known.Tests
{
    public sealed class Displayer
    {
        public static void Write(object message)
        {
            Console.Write(message);
        }

        public static void Write(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = orgColor;
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void WriteLine(object message)
        {
            Console.WriteLine(message);
        }

        public static void WriteLine(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = orgColor;
        }
    }
}
