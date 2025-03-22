namespace RequestValidationInMinimalAPIs.Endpoints;

public class HealthEndpoint
{
	private readonly ILogger<HealthEndpoint> _logger;

	public HealthEndpoint (ILogger<HealthEndpoint> logger) =>
		_logger = logger;

	public void MapHealthEndpoint (IEndpointRouteBuilder app)
	{
		app.MapGet ("/health", async (HttpContext context) =>
		{
			return await GetHealthAsync (context);
		})
			.WithOpenApi ()
			.RequireCors ()
			.RequireRateLimiting ("Concurrency")
			.RequireRateLimiting ("FixedWindow")
			.WithTags ("Health")
			.WithSummary ("Get health check for this application and it's database")
			.WithDescription ("This endpoint displays the health of your application along with the health of your database.")
			.Produces<HealthCheck> (StatusCodes.Status200OK)
			.Produces (StatusCodes.Status204NoContent)
			.Produces<string> (StatusCodes.Status500InternalServerError);
	}

	public async Task<Results<Ok<HealthCheck>, NoContent, InternalServerError<string>>> GetHealthAsync (HttpContext context)
	{
		try
		{
			var httpClient = context.RequestServices.GetService<IHttpClientFactory> ()?
				.CreateClient ("HealthClient");

			if (httpClient is null)
				return TypedResults.NoContent ();

			var httpResponse = await httpClient.GetAsync ("/healthCheck");

			if (!httpResponse.IsSuccessStatusCode)
				return TypedResults.NoContent ();

			var healthReport = await httpResponse.Content.ReadFromJsonAsync<HealthCheck> ();

			if (healthReport is null)
				return TypedResults.NoContent ();

			return TypedResults.Ok (healthReport);
		}
		catch (Exception ex)
		{
			_logger.LogError (ex, "An error occurred while checking health.");

			return TypedResults.InternalServerError ("An error occurred while checking health.");
		}
	}
}