using Known.Web;

KHost.Run(args, o =>
{
    o.DbFactories["MySqlConnector"] = typeof(MySqlConnector.MySqlConnectorFactory);
});