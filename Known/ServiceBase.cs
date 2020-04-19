using Known.Web;

namespace Known
{
    public class ServiceBase
    {
        private Database database;
        protected Database Database
        {
            get
            {
                if (database == null)
                    database = new Database();
                database.User = SessionHelper.GetUser();
                return database;
            }
        }
    }
}
