using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Known.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database();
            var db1 = new Database("SQLite");
            var db2 = new Database("Oracle");
            var db3 = new Database("MySql");

            Console.WriteLine("按任意键结束！");
            Console.ReadKey();
        }
    }
}
