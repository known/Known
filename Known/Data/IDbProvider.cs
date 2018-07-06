using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    public interface IDbProvider
    {
        string ProviderName { get; }
        string ConnectionString { get; }

        void Execute(Command command);
        void Execute(List<Command> commands);
        object Scalar(Command command);
        DataTable Query(Command command);
        void WriteTable(DataTable table);
    }
}
