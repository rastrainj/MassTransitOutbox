using MassTransit;
using MasstransitOutbox.Api.Diagnostics;
using MasstransitOutbox.Contracts;

namespace MasstransitOutbox.Api;

public class TestConsumer : IConsumer<Test>
{
	private readonly ILogger<TestConsumer> _logger;

	public TestConsumer(ILogger<TestConsumer> logger)
	{
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<Test> context)
	{
		_logger.Consuming(nameof(Test), context.Message.Date);

        await Task.Delay(1_000).ConfigureAwait(false);

        throw new NotSupportedException("my-exception");

        //_logger.Consumed(nameof(Test), context.Message.Date);
	}
}
