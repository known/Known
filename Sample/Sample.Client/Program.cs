using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample;
using Sample.Client;

Config.IsClient = true;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSample();
builder.Services.AddSampleClient();
await builder.Build().RunAsync();