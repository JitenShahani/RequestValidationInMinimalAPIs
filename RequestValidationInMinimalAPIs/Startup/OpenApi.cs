namespace RequestValidationInMinimalAPIs.Startup;

public static class OpenApi
{
	public static void ConfigureOpenApi (
		this IServiceCollection services,
		string httpsUrl,
		string httpUrl)
	{
		services.AddOpenApi (options =>
		{
			options.AddDocumentTransformer ((document, serviceProvider, cancellationToken) =>
			{
				document.Info.Version = "v1";
				document.Info.Title = "Request Validation in Minimal APIs.";
				document.Info.Description = "A sample application to demonstrate request validation in Minimal APIs";
				document.Info.Contact = new OpenApiContact
				{
					Name = "Jiten Shahani",
					Email = "shahani.jiten@gmail.com",
					Url = new Uri ("https://github.com/JitenShahani")
				};
				document.Info.License = new OpenApiLicense
				{
					Name = "MIT License",
					Url = new Uri ("https://opensource.org/licenses/MIT")
				};

				document.Tags =
				[
					new OpenApiTag
					{
						Name = "Posts",
						Description = "Minimal endpoints for creating, reading, updating, and deleting blog posts",
						ExternalDocs = new OpenApiExternalDocs
						{
							Description = "Documentation",
							Url = new Uri("https://github.com/JitenShahani/RequestValidationInMinimalAPIs")
						}
					},
					new OpenApiTag
					{
						Name = "Exception",
						Description = "Minimal endpoints that throws an exception if the argument value is empty"
					},
					new OpenApiTag
					{
						Name = "Health",
						Description = "Minimal endpoint to check health of this application and its database"
					}
				];

				document.Servers =
				[
					new OpenApiServer { Url = httpsUrl, Description = "Development Server - Https" },
					new OpenApiServer { Url = httpUrl, Description = "Development Server - Http" }
				];

				return Task.CompletedTask;
			});
		});
	}
}