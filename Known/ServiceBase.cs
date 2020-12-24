namespace Known
{
    public class ServiceBase
    {
        protected PlatformService Platform => new PlatformService();

        protected UserInfo CurrentUser
        {
            get { return UserHelper.GetUser(out _); }
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
