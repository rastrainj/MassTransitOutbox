using MassTransit;
using MasstransitOutbox.Api.Data;

namespace MasstransitOutbox.Api;

public class WeatherForecastCreatedConsumerDefinition : ConsumerDefinition<WeatherForecastCreatedConsumer>
{
	private readonly IServiceProvider _serviceProvider;

	public WeatherForecastCreatedConsumerDefinition(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<WeatherForecastCreatedConsumer> consumerConfigurator)
	{
		base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
		endpointConfigurator.UseEntityFrameworkOutbox<MyContext>(_serviceProvider);
	}
}
