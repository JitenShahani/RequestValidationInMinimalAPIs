namespace RequestValidationInMinimalAPIs.Startup;

public static class IoC
{
	public static void ConfigureIoCContainer (this WebApplicationBuilder builder)
	{
		var httpUrl = builder.Configuration["ServerInfo:HttpUrl"]!;
		var httpsUrl = builder.Configuration["ServerInfo:HttpsUrl"]!;
		var antiForgeryToken = builder.Configuration["AntiForgeryToken:HeaderName"]!;

		// Configure DI Container to blow up early at build time instead of run time.
		builder.Host.UseDefaultServiceProvider ((context, options) =>
		{
			// Validate that scoped services are not directly or indirectly resolved from singleton services.
			options.ValidateScopes = true;

			// Validate the service provider during builder.Build() to detect any configuration issues.
			// Remember, you will have to add all required services to a service so the instance can be created.
			options.ValidateOnBuild = true;
		});

		// Configure logging
		Log.Logger = new LoggerConfiguration ()
			.WriteTo.File ("../Logs/RequestValidationInMinimalAPIs-.log", rollingInterval: RollingInterval.Day)
			.CreateLogger ();

		builder.Logging.ClearProviders ();
		builder.Logging.AddSimpleConsole ();
		builder.Logging.AddSerilog ();

		// Configure ApiExplorer using Endpoint Metadata
		builder.Services.AddEndpointsApiExplorer ();

		// Configure Antiforgery
		builder.Services.AddAntiforgery (options =>
		{
			options.HeaderName = antiForgeryToken;
		});

		// Configure HttpClient
		builder.Services.AddHttpClient ("HealthClient", client =>
		{
			client.BaseAddress = new Uri (httpsUrl);
			client.Timeout = TimeSpan.FromSeconds (5);
		})
			.AddHttpMessageHandler<LoggingHandler> ();

		// Enable Controllers
		builder.Services.AddControllers ();

		// Configure Health Checks
		builder.Services.AddHealthChecks ()
			.AddCheck<DatabaseHealthCheck> ("Database");

		// Configure ProblemDetails
		builder.Services.AddProblemDetails (options =>
		{
			options.CustomizeProblemDetails = context =>
			{
				context.ProblemDetails.Instance =
					$"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

				context.ProblemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier;
			};
		});

		// Configure OpenApi Document
		builder.Services.ConfigureOpenApi (httpsUrl, httpUrl);

		// Configure CORS
		/*
            Remember, following Http headers will be set for CORS based on my code below...
            1. 'Access-Control-Allow-Origin': 'httpsUrl; httpUrl'
            2. 'Access-Control-Allow-Methods': '*'
            3. 'Access-Control-Allow-Headers': '*'
            4. 'Access-Control-Allow-Credentials': '' - I am not using credentials in my requests
        */
		builder.Services.AddCors (options =>
		{
			options.AddDefaultPolicy (builder =>
			{
				builder
					.WithOrigins (httpsUrl, httpUrl)
					.AllowAnyMethod ()
					.AllowAnyHeader ();
			});
		});

		// Configure Rate Limiter(s)
		builder.Services.AddRateLimiter (rateLimiterOptions =>
		{
			rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			rateLimiterOptions.AddFixedWindowLimiter ("FixedWindow", options =>
			{
				options.PermitLimit = 2;
				options.Window = TimeSpan.FromSeconds (5);
				options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				options.QueueLimit = 0;
			});

			rateLimiterOptions.AddSlidingWindowLimiter ("SlidingWindow", options =>
			{
				options.PermitLimit = 2;
				options.Window = TimeSpan.FromSeconds (5);
				options.SegmentsPerWindow = 2;
				options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				options.QueueLimit = 0;
			});

			rateLimiterOptions.AddTokenBucketLimiter ("TokenBucket", options =>
			{
				options.TokenLimit = 2;
				options.TokensPerPeriod = 2;
				options.ReplenishmentPeriod = TimeSpan.FromSeconds (5);
				options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				options.QueueLimit = 0;
				options.AutoReplenishment = true;
			});

			rateLimiterOptions.AddConcurrencyLimiter ("Concurrency", options =>
			{
				options.PermitLimit = 2;
				options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				options.QueueLimit = 0;
			});
		});

		// Register Hybrid Cache
		builder.Services.AddHybridCache (options =>
		{
			options.MaximumPayloadBytes = 1024 * 1024;
			options.MaximumKeyLength = 1024;
			options.DefaultEntryOptions = new HybridCacheEntryOptions
			{
				Expiration = TimeSpan.FromMinutes (5),
				LocalCacheExpiration = TimeSpan.FromMinutes (5)
			};
		});

		// Register ServiceValidator service.
		// This service injects all the required services to make sure that exception(s) are reaised in case they are unregistered.
		builder.Services.AddSingleton<ServiceValidator> ();

		// Register Logging Handler for HttpClient
		builder.Services.AddSingleton<LoggingHandler> ();

		// Register Database Service
		builder.Services.AddSingleton<Database> ();

		// Register all the Validators from this assembly - Fluent Validators
		builder.Services.AddValidatorsFromAssemblyContaining<Program> ();
	}
}