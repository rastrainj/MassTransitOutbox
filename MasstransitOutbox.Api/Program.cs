// CA1852 Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CA1852

using MassTransit;

using MasstransitOutbox.Api;
using MasstransitOutbox.Api.Data;
using MasstransitOutbox.Api.Data.Models;
using MasstransitOutbox.Api.Diagnostics;
using MasstransitOutbox.Contracts;

using Microsoft.EntityFrameworkCore;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyContext>(options =>
{
    //options.EnableSensitiveDataLogging();
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddEntityFrameworkOutbox<MyContext>(outboxConfigurator =>
    {
        outboxConfigurator
            .UseSqlServer()
            .UseBusOutbox();

        outboxConfigurator.DisableInboxCleanupService();

        outboxConfigurator.QueryMessageLimit = 10;
        outboxConfigurator.QueryDelay = TimeSpan.FromSeconds(5);
        outboxConfigurator.QueryTimeout = TimeSpan.FromSeconds(2);
    });

    busConfigurator.AddConsumersFromNamespaceContaining<WeatherForecastCreatedConsumer>();    

    busConfigurator.SetKebabCaseEndpointNameFormatter();

    //busConfigurator.UsingInMemory((busContext, transportConfigurator) =>
    //{
    //	transportConfigurator.ConfigureEndpoints(busContext);
    //});

    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resourceBuilder =>
    {
        resourceBuilder.AddService(
            "MassTransitOutbox",
            serviceVersion: "0.0.1",
            serviceInstanceId: Environment.MachineName);
    })
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
            .SetSampler(new AlwaysOnSampler())
            .AddAspNetCoreInstrumentation(options => options.RecordException = true)
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
            .AddConsoleExporter()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "localhost";
                options.AgentPort = 6831;
                options.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
            });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateAsyncScope();
var context = scope.ServiceProvider.GetRequiredService<MyContext>();
//await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
await scope.DisposeAsync().ConfigureAwait(false);

app.MapPost(
    "/weatherforecast",
    async (
        WeatherForecastRequest request,
        MyContext context,
        IPublishEndpoint publishEndpoint,
        ILogger<Program> logger,
        CancellationToken ct) =>
    {
        var weatherForecast = new WeatherForecast
        {
            Id = NewId.NextGuid(),
            Date = request.Date,
            TemperatureC = request.TemperatureC,
            Summary = request.Summary
        };

        await context.AddAsync(weatherForecast, ct).ConfigureAwait(false);

        await publishEndpoint.Publish(
            new WeatherForecastCreated(weatherForecast.Id, weatherForecast.Date, weatherForecast.TemperatureC, weatherForecast.Summary),
            ct).ConfigureAwait(false);

        await publishEndpoint.Publish(
            new Test(DateTime.Now),
            ct).ConfigureAwait(false);

        logger.Saving();

        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        logger.Saved();

        return TypedResults.Ok(weatherForecast.Id);
    })
    .WithName("PostWeatherForecast")
    .WithOpenApi();

await app.RunAsync().ConfigureAwait(false);
