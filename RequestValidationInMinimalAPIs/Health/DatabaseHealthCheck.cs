namespace RequestValidationInMinimalAPIs.Health;

public class DatabaseHealthCheck : IHealthCheck
{
	private readonly Database _database;

	public DatabaseHealthCheck (Database database) =>
		_database = database;

	public async Task<HealthCheckResult> CheckHealthAsync (
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		await Task.Delay (0, cancellationToken);

		try
		{
			bool databaseIsHealthy = _database.Posts is not null && _database.Posts.Count > 0;

			if (databaseIsHealthy)
				return HealthCheckResult.Healthy ($"{_database.Posts!.Count.ToString ()} Record found!");

			return HealthCheckResult.Unhealthy ("The database is unhealthy.");
		}
		catch (Exception ex)
		{
			return HealthCheckResult.Unhealthy ("An error occurred while checking the database health.", ex);
		}
	}
}