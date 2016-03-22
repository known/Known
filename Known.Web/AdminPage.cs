namespace Known.Web
{
    public class AdminPage : PageBase
    {
        public override string VirtualPath
        {
            //get { return KConfig.AdminPath; }
            get { return ""; }
        }

        protected override void ValidateLogin()
        {
            //if (AppContext.CurrentUser == null)
            //{
            //    var url = Utils.GetAdminUrl("Login.aspx");
            //    Response.Navigate(url);
            //}
        }
    }
}
