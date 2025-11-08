using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Known.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApplicationClient(option =>
{
    option.BaseAddress = builder.HostEnvironment.BaseAddress + "api";
});

await builder.Build().RunAsync();