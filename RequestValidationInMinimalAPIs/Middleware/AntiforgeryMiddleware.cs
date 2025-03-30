namespace RequestValidationInMinimalAPIs.Middleware;

public class AntiforgeryMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IAntiforgery _antiforgery;
	private readonly string _headerName;
	private string _methodName;

	public AntiforgeryMiddleware (
		RequestDelegate next,
		IAntiforgery antiforgery,
		IConfiguration configuration)
	{
		_next = next;
		_antiforgery = antiforgery;
		_headerName = configuration["AntiForgeryToken:HeaderName"]!;
		_methodName = string.Empty;
	}

	public async Task InvokeAsync (HttpContext context)
	{
		_methodName = context.Request.Method;

		if (HttpMethods.IsPost (_methodName) || HttpMethods.IsPut (_methodName) ||
			HttpMethods.IsPatch (_methodName) || HttpMethods.IsDelete (_methodName))
		{
			try
			{
				await _antiforgery.ValidateRequestAsync (context);
			}
			catch (AntiforgeryValidationException)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				context.Response.ContentType = "application/json";

				ProblemDetailsFactory problemDetailsFactory =
					context.RequestServices.GetRequiredService<ProblemDetailsFactory> ();

				ProblemDetails problemDetails = problemDetailsFactory
					.CreateProblemDetails (context, StatusCodes.Status400BadRequest);

				Dictionary<string, object?> error = new ()
							{
								{ "message", "Invalid anti-forgery token." },
								{ "tokenReceived", context.Request.Headers[_headerName].ToString() }
							};

				problemDetails.Extensions["errors"] = error;

				await context.Response.WriteAsJsonAsync (problemDetails);
				return;
			}
		}

		await _next (context);
	}
}