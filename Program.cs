using DMS.Components.DeadManSwitch;
using DMS.Components.Login;
using DMS.Configurations;
using DMS.DataProviders.Login;
using DMS.Logging;
using DMS.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddSingleton<ILogger, ConsoleJsonLogger>();

// configurations
builder.Services.AddSingleton<IEnvironmentConfig, EnvironmentConfig>();
// components
builder.Services.AddScoped<IDeadManSwitchComponent, DeadManSwitchComponent>();
builder.Services.AddScoped<ILoginComponent, LoginComponent>();
// repositories
builder.Services.AddSingleton<ILoginRepository, LoginRepository>();

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

await app.RunAsync();