namespace RequestValidationInMinimalAPIs.Startup;

public static class ConfigureEndpoints
{
	public static void MapMyEndpoints (this WebApplication app, IConfiguration configuration)
	{
		var loggerFactory = app.Services.GetRequiredService<ILoggerFactory> ();
		var logger = loggerFactory.CreateLogger<HealthEndpoint> ();

		new PostEndpoints (configuration).MapPostEndpoints (app);
		new HealthEndpoint (logger).MapHealthEndpoint (app);
	}
}