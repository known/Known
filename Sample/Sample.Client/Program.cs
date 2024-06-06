using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddClient();
await builder.Build().RunAsync();