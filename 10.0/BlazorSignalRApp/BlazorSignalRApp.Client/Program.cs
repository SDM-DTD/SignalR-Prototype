using DTOs;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TheData>();
await builder.Build().RunAsync();
