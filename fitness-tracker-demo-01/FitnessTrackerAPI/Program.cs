using FitnessTracker.Common.Utils;
using FitnessTrackerAPI.Services;
using FitnessTrackerClient.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ICryptoServerManager, CryptoServerManager>();

builder.Services.Configure<FitnessCryptoConfig>(options =>
    {
        options.PolyModulusDegree = SEALUtils.DEFAULTPOLYMODULUSDEGREE;
    });

builder.Services
    .AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();

app.UseCors();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();


namespace FitnessTrackerAPI
{
    // Make the implicit Program class public so test projects can access it
    public partial class Program { }
}