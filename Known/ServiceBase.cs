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
                database.User = CurrentUser;
                return database;
            }
        }

        protected UserInfo CurrentUser
        {
            get { return SessionHelper.GetUser(out _); }
        }
    }
}
