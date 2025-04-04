namespace RequestValidationInMinimalAPIs.ClientHandlers;

public class LoggingHandler : DelegatingHandler
{
	private readonly ILogger<LoggingHandler> _logger;

	public LoggingHandler (ILogger<LoggingHandler> logger) =>
		_logger = logger;

	protected override async Task<HttpResponseMessage> SendAsync (
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		_logger.LogInformation ("***Sending request to {requestUrl}", request.RequestUri);
		var startTime = Stopwatch.GetTimestamp ();

		// Log request body
		if (request.Content is not null)
		{
			var requestBody = await request.Content.ReadAsStringAsync (cancellationToken);
			_logger.LogInformation ("***Request Body: {requestBody}", requestBody);
		}

		HttpResponseMessage response = await base.SendAsync (request, cancellationToken);

		var delta = Stopwatch.GetElapsedTime (startTime);

		_logger.LogInformation (
			"***Received response from {requestUrl} in {elapsedMilliseconds}ms",
			request.RequestUri,
			(delta.TotalMicroseconds / 1000));

		// Log response body
		if (response.Content is not null)
		{
			var responseBody = await response.Content.ReadAsStringAsync ();
			_logger.LogInformation ("***Response Body: {responseBody}", responseBody);
		}

		return response;
	}
}