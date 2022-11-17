using FitnessTracker.Common.Utils;
using FitnessTrackerAPI;
using FitnessTrackerAPI.Services;
using FitnessTrackerClient.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(typeof(CryptoServerManagerCKKS));
builder.Services.AddSingleton(typeof(CryptoServerManagerBGV));


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

app.MapGet("/", () => "Welcome to the Fitness Tracked Sample");

app.MapPost("/api/bgv/keys", CryptoServices.SavePublicKeyBGV);
app.MapPost("/api/bgv/metrics", CryptoServices.SaveRunBGV);
app.MapGet("/api/bgv/metrics", CryptoServices.GetMetricsBGV);

app.MapPost("/api/ckks/keys", CryptoServices.SavePublicKeyCKKS);
app.MapPost("/api/ckks/metrics", CryptoServices.SaveRunCKKS);
app.MapGet("/api/ckks/metrics", CryptoServices.GetMetricsCKKS);

app.Run();

namespace FitnessTrackerAPI
{
    // Make the implicit Program class public so test projects can access it
    public partial class Program { }
}