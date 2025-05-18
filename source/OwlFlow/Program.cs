using System.Text.Json;
using OwlFlow.Model;
using OwlFlow.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var serviceServerRepository = new ServiceJsonSerializerServers() { PathRepository = new FileInfo("wwwroot\\data\\servers.json") };
var serviceRepo = new ServiceRepository(serviceServerRepository);
var servers = serviceRepo.GetServers();
//Singleton service 
builder.Services.AddSingleton<ServiceJsonSerializerServers>(serviceServerRepository);
builder.Services.AddSingleton<ServiceRepository>(serviceRepo);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
