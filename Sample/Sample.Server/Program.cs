using Sample.Server;

var builder = WebApplication.CreateBuilder(args);
AppServer.Run(builder);