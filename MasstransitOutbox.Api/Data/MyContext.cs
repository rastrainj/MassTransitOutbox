using MassTransit;
using MasstransitOutbox.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MasstransitOutbox.Api.Data;

public class MyContext : DbContext
{
	public MyContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.AddInboxStateEntity();
		modelBuilder.AddOutboxMessageEntity();
		modelBuilder.AddOutboxStateEntity();

		var entityBuilder = modelBuilder.Entity<WeatherForecast>();

		entityBuilder.ToTable("WeatherForecasts");
		entityBuilder.HasKey(x => x.Id);
	}
}
