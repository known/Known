using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Known.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            var db = new Database();
            var db1 = new Database("SQLite");
            var db2 = new Database("Oracle");
            var db3 = new Database("MySql");

            Console.WriteLine("按任意键结束！");
            Console.ReadKey();
        }
    }
}
