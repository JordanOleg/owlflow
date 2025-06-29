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

builder.Services.AddDistributedMemoryCache(); // Реалізація сховища для сесій
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();

app.UseRedirectRequestMiddleware(); // Custom Redirect request Middleware

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
