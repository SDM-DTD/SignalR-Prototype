using BlazorSignalRApp;
using BlazorSignalRApp.Client;
using BlazorSignalRApp.Components;
using BlazorSignalRApp.Hubs;
using DTOs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

builder.Services.AddScoped<ChatHub>();
builder.Services.AddScoped<ClientManager>();
var app = builder.Build();
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorSignalRApp.Client._Imports).Assembly);

app.MapHub<ChatHub>("/chathub");

using (var scope = app.Services.CreateScope())
{
    var clientManager = scope.ServiceProvider.GetRequiredService<ClientManager>();
    new DbWatcher(app.Services.GetService<IHubContext<ChatHub>>(), clientManager).Start();
}


app.Run();
