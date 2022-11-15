using FitnessTracker.Common.Utils;
using FitnessTrackerAPI.Services;
using FitnessTrackerClient.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Research.SEAL;
using System;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton<ICryptoServerManager, CryptoServerManager>();


builder.Services.AddSingleton(typeof(CryptoServerManagerCKKS));
builder.Services.AddSingleton(typeof(CryptoServerManagerBGV));
builder.Services.AddSingleton(typeof(CryptoServerManagerBFV));



//builder.Services.AddSingleton<ICryptoServerManager>(sp => new CryptoServerManager(SEALUtils.DEFAULTPOLYMODULUSDEGREE, SchemeType.BFV));


builder.Services.AddSingleton<Func<SchemeType, ICryptoServerManager>>(serviceProvider => key =>
{
   
    ICryptoServerManager cryptoManager = null;

    switch (key)
    {
        case SchemeType.BFV:
            cryptoManager = serviceProvider.GetRequiredService<CryptoServerManagerBFV>();
            break;
        case SchemeType.BGV:
            cryptoManager = serviceProvider.GetRequiredService<CryptoServerManagerBGV>();
            break;
        case SchemeType.CKKS:
            cryptoManager = serviceProvider.GetRequiredService<CryptoServerManagerCKKS>();
            break;
    }

    return cryptoManager;
});


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