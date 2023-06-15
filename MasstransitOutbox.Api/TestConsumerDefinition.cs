using MassTransit;
using MasstransitOutbox.Api.Data;

namespace MasstransitOutbox.Api;

public class TestConsumerDefinition : ConsumerDefinition<TestConsumer>
{
	private readonly IServiceProvider _serviceProvider;

	public TestConsumerDefinition(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TestConsumer> consumerConfigurator)
	{
		base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
		endpointConfigurator.UseEntityFrameworkOutbox<MyContext>(_serviceProvider);
	}
}
