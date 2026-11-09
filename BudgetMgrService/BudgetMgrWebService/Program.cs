using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Processors;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

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
builder.Services.AddOpenApi();

builder.Services.AddOptions();
builder.Services.AddDbContext<TransactionContext>(opt => opt.UseInMemoryDatabase("transactions"));
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
    var context = scope.ServiceProvider.GetRequiredService<TransactionContext>();
    context.SeedWithData(app.Environment.ContentRootPath);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
