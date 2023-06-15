using System.Text.Json;

using MassTransit;

using MasstransitOutbox.Contracts;

namespace MasstransitOutbox.Consumer.Console;

public class MyConsoleConsumer : IConsumer<WeatherForecastCreated>
{
    public Task Consume(ConsumeContext<WeatherForecastCreated> context)
    {
        var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });

        System.Console.WriteLine($"WeatherForecastCreated event consumed. Message: {serializedMessage}");

        return Task.CompletedTask;
    }
}