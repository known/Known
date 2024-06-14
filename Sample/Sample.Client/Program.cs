using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Client;

Config.IsClient = true;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<IAuthStateProvider, ClientAuthStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentStateProvider>();
builder.Services.AddSampleClient();
await builder.Build().RunAsync();