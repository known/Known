using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample;
using Sample.Client;

Config.IsClient = true;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
Config.BaseUrl = builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient();
builder.Services.AddSample();
builder.Services.AddSampleClient();
await builder.Build().RunAsync();