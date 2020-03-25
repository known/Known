using System.Data.Common;

namespace Known
{
    public class BaseService
    {
        private DbConnection connection;
        protected DbConnection Connection
        {
            get
            {
                if (connection == null)
                    connection = DbHelper.CreateConnection();
                return connection;
            }
        }
    }
}
