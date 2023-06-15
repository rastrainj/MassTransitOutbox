namespace MasstransitOutbox.Api;

public sealed record WeatherForecastRequest(DateTime Date, int TemperatureC, string? Summary);
