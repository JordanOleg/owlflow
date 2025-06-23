using System.Text.Json;
using OwlFlow.Models;
using OwlFlow.Service;
using OwlFlow.Service.Background;
using OwlFlow.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var serviceServerRepository = new ServiceJsonSerializerServers() { PathRepository = new FileInfo("wwwroot\\data\\servers.json") };
var serviceRepo = new ServiceRepository(serviceServerRepository);
var servers = serviceRepo.GetServers();
//Service 
builder.Services.AddSingleton<ServiceJsonSerializerServers>(serviceServerRepository);
builder.Services.AddSingleton<ServiceRepository>(serviceRepo);
builder.Services.AddSingleton<ServiceSelectServer>();

builder.Services.AddSingleton<ServiceCheckerTryConnection>();

builder.Services.AddHostedService<ServiceServersChecker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePages();
app.UseHttpsRedirection();

//app.UseRedirectRequestMiddleware(); // Custom Redirect request Middleware

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
