using BudgetMgrServiceLib.Models;
using BudgetMgrWebService;
using BudgetMgrWebService.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("testdatasettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("grouptypesettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

builder.Services.Configure<TestDataCfg>(builder.Configuration.GetSection("TestDataCfg"));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddTransient<BearerSecuritySchemeTransformer>();
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info = new OpenApiInfo
        {
            Title = "Budget Mgr Web API",
            Version = "1.0",
            Description = "Budget Calculation API.",
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddOptions();
builder.Services.AddDbContext<DataStore>(opt => opt.UseInMemoryDatabase("transactions"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
                      builder => builder.WithOrigins("http://localhost:7261")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataStore = scope.ServiceProvider.GetRequiredService<DataStore>();
    dataStore.SeedWithData(app.Environment.ContentRootPath);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Serves /openapi/v1.json
    app.MapOpenApi("/openapi/v1.json");

    // Serves Scalar UI at /scalar
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
