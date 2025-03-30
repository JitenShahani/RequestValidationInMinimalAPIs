namespace RequestValidationInMinimalAPIs.Startup;

public static class Middleware
{
	public static void ConfigureMiddleware (this WebApplication app, IConfiguration configuration)
	{
		// Enable global exception handling middleware
		app.UseMiddleware<ExceptionHandlingMiddleware> ();

		// Enable access to the static assets like CSS, JS, and images
		app.MapStaticAssets ();

		// Enable status code pages for better error responses
		app.UseStatusCodePages ();

		// Enable CORS early in the pipeline
		app.UseCors ();

		// Enable HTTPS redirection in non-development environments
		if (!app.Environment.IsDevelopment ())
			app.UseHttpsRedirection ();

		// Map controllers
		app.MapControllers ();

		// Use Anti forgery
		app.UseAntiforgery ();

		// Map health checks
		app.MapHealthChecks ("/healthCheck", new HealthCheckOptions
		{
			ResponseWriter = async (context, report) =>
			{
				context.Response.ContentType = "application/json";

				var result = JsonSerializer.Serialize (new
				{
					apiStatus = report.Status.ToString (),
					checks = report.Entries.Select (entry => new
					{
						name = entry.Key,
						status = entry.Value.Status.ToString (),
						description = entry.Value.Description
					})
				});

				await context.Response.WriteAsync (result);
			}
		})
			.WithOpenApi ()
			.RequireCors ()
			.DisableRateLimiting ()
			.WithTags ("Health");

		// Map OpenAPI endpoints
		app.MapOpenApi ();

		// Configure Swagger UI
		app.UseSwaggerUI (options =>
		{
			// Add Document Title for Swagger UI
			options.DocumentTitle = "Request Validation in Minimal APIs v1.";

			// Collapse all the tags & schema sections.
			options.DocExpansion (DocExpansion.None);

			// Enable deep linking for tags and operations in the URL.
			options.EnableDeepLinking ();

			// Enable filtering of the operations.
			options.EnableFilter ();

			// Enable the validator badge.
			options.EnableValidator ();

			// Enable the "Try it out" button out of the box.
			options.EnableTryItOutByDefault ();

			// Display OperationId for all endpoints.
			options.DisplayOperationId ();

			// Display the request duration at the end of the response.
			options.DisplayRequestDuration ();

			// Render example in the Model tab.
			options.DefaultModelRendering (ModelRendering.Example);

			// Set the default model expand depth to 4.
			options.DefaultModelExpandDepth (4);

			// Set the default model expand depth to 4.
			options.DefaultModelsExpandDepth (4);

			// Define the Swagger endpoints.
			options.SwaggerEndpoint (
				"/openApi/v1.json",
				"Request Validation in Minimal APIs v1");

			// Apply custom css styles to Swagger UI.
			options.InjectStylesheet ("/css/CustomSwaggerUI.css");

			// Apply custom css style to enable Dark Mode in Swagger UI
			options.InjectStylesheet ("/css/SwaggerDarkMin.css");
		});

		// Enable rate limiting
		app.UseRateLimiter ();

		// Use Anti forgery middleware
		app.UseMiddleware<AntiforgeryMiddleware> ();

		// Map all my endpoints
		app.MapMyEndpoints (configuration);

		// Map the Anti Forgery Token endpoint
		app.MapGet ("/aft", ([FromServices] IAntiforgery antiforgery, HttpContext httpContext) =>
		{
			string token = antiforgery.GetAndStoreTokens (httpContext).RequestToken!;
			string headerName = configuration["AntiForgeryToken:HeaderName"]!;
			httpContext.Response.Headers.Append (headerName, token);

			return TypedResults.Ok (new AFToken () { Token = token });
		})
			.WithOpenApi ()
			.RequireCors ()
			.DisableRateLimiting ()
			.WithSummary ("Get Anti Forgery Token")
			.WithDescription ("This endpoint returns the Anti Forgery Token.")
			.Produces<AFToken> (StatusCodes.Status200OK)
			.ExcludeFromDescription ()
			.WithTags ("AFT");

		// Map the Exception endpoint
		app.MapGet ("/exception", ([FromQuery] string? Name) =>
		{
			if (string.IsNullOrEmpty (Name))
				throw new ArgumentNullException (nameof (Name));

			return TypedResults.Ok (Name);
		})
			.WithOpenApi ()
			.RequireCors ()
			.DisableRateLimiting ()
			.WithSummary ("Throws an exception if the name argument is empty")
			.WithDescription ("This endpoint throws an exception if the name argument is empty.")
			.Produces<string> (StatusCodes.Status200OK)
			.ProducesProblem (StatusCodes.Status500InternalServerError)
			.WithTags ("Exception");
	}
}