using Sample.Web;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
                //.AddInteractiveWebAssemblyComponents();
builder.AddApplication();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();
app.UseApplication();
app.MapRazorPages();
app.MapRazorComponents<App>()   
   .AddInteractiveServerRenderMode()
   //.AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies([.. Config.Assemblies]);
app.Run();