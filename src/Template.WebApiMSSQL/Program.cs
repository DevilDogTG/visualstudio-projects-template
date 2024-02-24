using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Template.WebApiMSSQL.Constraints;
using Template.WebApiMSSQL.Middlewares;
using Template.WebApiMSSQL.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection(Const.ConfigurationKey).Get<ConfigurationModel>() ?? throw new ArgumentNullException("Configuration not found.");
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext());

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure Performance Logging
var perfLog = new LoggerConfiguration()
    .WriteTo.File(
        path: config.PerformanceLog.Path,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Message:lj}{NewLine}")
    .CreateLogger();
var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(perfLog));
var perfLogger = loggerFactory.CreateLogger<PerformanceLogMiddleware>();
builder.Services.AddSingleton(perfLogger);
//Configure Services
builder.Services.AddSingleton<IConfigurationModel, ConfigurationModel>(sp => config);


var app = builder.Build();

app.UseMiddleware<PerformanceLogMiddleware>();

//Configure the HTTP request pipeline.
if (config.EnableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
