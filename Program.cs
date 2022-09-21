using DMS;
using DMS.Configurations;
using DMS.DataProviders.DataFactory;
using DMS.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder
    .Services
    .AddConfig()
    .AddLoggers()
    .AddComponents()
    .AddSwagger();

builder.Services.AddControllers();

var app = builder.Build();

ILogger logger = app.Services.GetRequiredService<ILogger>();
IEnvironmentConfig envConfig = app.Services.GetRequiredService<IEnvironmentConfig>();
app.ConfigureExceptionHandler(logger, envConfig);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Async operations should not run in constructors invoked by dependency injection; they can lead to a deadlock, see this article
// https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
var dataFactoryService = app.Services.GetRequiredService<IDataFactoryService>();
await dataFactoryService.Initialize();

await app.RunAsync();