namespace MasstransitOutbox.Api.Diagnostics;

public static partial class Log
{
	[LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Consuming {type} {date}.")]
	public static partial void Consuming(
		this ILogger logger,
		string type,
		DateTime date);

	[LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Consumed {type} {date}.")]
	public static partial void Consumed(
		this ILogger logger,
		string type,
		DateTime date);

	[LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Saving...")]
	public static partial void Saving(this ILogger logger);

	[LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Saved!!!")]
	public static partial void Saved(this ILogger logger);
}
