using BlazorSignalRApp;
using BlazorSignalRApp.Components;
using BlazorSignalRApp.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .WithOrigins("https://localhost:7037")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<ChatHub>();
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
app.MapRazorComponents<App>();
app.UseCors("CorsPolicy");
app.MapHub<ChatHub>("/chathub");

using (var scope = app.Services.CreateScope())
{
    new DbWatcher(app.Services.GetService<IHubContext<ChatHub>>()).Start();
}

app.Run();
