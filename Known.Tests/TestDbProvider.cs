using System.Data;
using Known.Data;

namespace Known.Tests
{
    class TestDbProvider : IDbProvider
    {
        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void BeginTrans()
        {
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public void Execute(Command command)
        {
        }

        public object Scalar(Command command)
        {
            return "";
        }

        public DataTable Query(Command command)
        {
            return new DataTable();
        }

        public void WriteTable(DataTable table)
        {
        }

        public void Dispose()
        {
        }
    }
}
