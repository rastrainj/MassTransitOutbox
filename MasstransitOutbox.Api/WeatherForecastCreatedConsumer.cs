using MassTransit;
using MasstransitOutbox.Api.Diagnostics;
using MasstransitOutbox.Contracts;

namespace MasstransitOutbox.Api;

public class WeatherForecastCreatedConsumer : IConsumer<WeatherForecastCreated>
{
	private readonly ILogger<WeatherForecastCreatedConsumer> _logger;

	public WeatherForecastCreatedConsumer(ILogger<WeatherForecastCreatedConsumer> logger)
	{
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<WeatherForecastCreated> context)
	{
		_logger.Consuming(nameof(WeatherForecastCreated), context.Message.Date);
		await Task.Delay(15_000).ConfigureAwait(false);
		_logger.Consumed(nameof(WeatherForecastCreated), context.Message.Date);
	}
}
