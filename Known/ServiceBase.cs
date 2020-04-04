using System.Web;

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
                database.UserName = HttpContext.Current.User.Identity.Name;
                return database;
            }
        }
    }
}
