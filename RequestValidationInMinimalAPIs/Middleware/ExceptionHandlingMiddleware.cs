namespace RequestValidationInMinimalAPIs.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware (
		RequestDelegate next,
		ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync (HttpContext context)
	{
		try
		{
			await _next (context);
		}
		catch (Exception ex)
		{
			_logger.LogError (ex, "An unhandled exception occurred.");
			await HandleExceptionAsync (context, ex);
		}
	}

	private static Task HandleExceptionAsync (HttpContext context, Exception exception)
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

		return context.Response.WriteAsJsonAsync (problemDetails);
	}
}