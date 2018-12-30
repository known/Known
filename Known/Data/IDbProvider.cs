using System;
using System.Data;

namespace Known.Data
{
    public interface IDbProvider : IDisposable
    {
        string ProviderName { get; }
        string ConnectionString { get; }

        void BeginTrans();
        void Commit();
        void Rollback();
        void Execute(Command command);
        object Scalar(Command command);
        DataTable Query(Command command);
        void WriteTable(DataTable table);
    }
}
