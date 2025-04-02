namespace RequestValidationInMinimalAPIs.Middleware;

public class ExceptionHandling
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandling> _logger;

	public ExceptionHandling (
		RequestDelegate next,
		ILogger<ExceptionHandling> logger) =>
			(_next, _logger) = (next, logger);

	public async Task InvokeAsync (HttpContext context)
	{
		try
		{
			await _next (context);
		}
		catch (Exception ex)
		{
			_logger.LogError (ex, "An unhandled exception occurred.");
			await HandleExceptionAsync (context, ex, _logger);
		}
	}

	private static Task HandleExceptionAsync (HttpContext context, Exception exception, ILogger<ExceptionHandling> logger)
	{
		context.Response.StatusCode = StatusCodes.Status500InternalServerError;
		context.Response.ContentType = "application/problem+json";

		ProblemDetailsFactory problemDetailsFactory =
					context.RequestServices.GetRequiredService<ProblemDetailsFactory> ();

		ProblemDetails problemDetails = problemDetailsFactory
			.CreateProblemDetails (context, StatusCodes.Status500InternalServerError, "An unexpected error occurred!");

		Dictionary<string, object?> error = new ()
		{
			{ "message", exception.Message }
		};

		problemDetails.Extensions["errors"] = error;

		// Serialize ProblemDetails to JSON
		string problemDetailsJson = JsonSerializer.Serialize (
			problemDetails,
			new JsonSerializerOptions { WriteIndented = true 	});

		// Log the ProblemDetails JSON
		logger.LogError ("ProblemDetails: {ProblemDetailsJson}", problemDetailsJson);

		return context.Response.WriteAsJsonAsync (problemDetails);
	}
}