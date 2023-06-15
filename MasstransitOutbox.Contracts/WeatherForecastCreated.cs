namespace MasstransitOutbox.Contracts;

public sealed record WeatherForecastCreated(
	Guid WeatherForecastId,
	DateTime Date,
	int TemperatureC,
	string? Summary);
