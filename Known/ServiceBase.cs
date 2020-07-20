using Known.Web;

namespace Known
{
    public class ServiceBase
    {
        protected PlatformService Platform { get; } = new PlatformService();

        private UserInfo user;
        protected UserInfo CurrentUser
        {
            get
            {
                if (user != null)
                    return user;
                return UserHelper.GetUser(out _);
            }
            set { user = value; }
        }

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
    }
}
