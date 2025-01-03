using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddApplication();
builder.Services.AddKnownClient(option =>
{
    option.BaseAddress = builder.HostEnvironment.BaseAddress+ "api";
});

await builder.Build().RunAsync();