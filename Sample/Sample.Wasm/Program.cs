using Known;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddKnown(info =>
{
    info.Id = "AppId";
    info.Name = "AppName";
    info.Assembly = typeof(Program).Assembly;
});
builder.Services.AddKnownClient(option => option.BaseAddress = builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();