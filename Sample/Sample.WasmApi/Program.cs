using Sample.Wasm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddKnownWeb(option =>
{
    option.App.WebRoot = builder.Environment.WebRootPath;
    option.App.ContentRoot = builder.Environment.ContentRootPath;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseKnown();
app.MapFallbackToFile("index.html");

app.Run();
