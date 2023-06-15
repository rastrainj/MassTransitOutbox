using MassTransit;

using MasstransitOutbox.Contracts;

namespace MasstransitOutbox.Consumer.Console;

public class MyConsoleTestConsumer : IConsumer<Test>
{
    public Task Consume(ConsumeContext<Test> context)
    {
        System.Console.WriteLine($"Test consumido {context.Message.Date}");

        return Task.CompletedTask;
    }
}