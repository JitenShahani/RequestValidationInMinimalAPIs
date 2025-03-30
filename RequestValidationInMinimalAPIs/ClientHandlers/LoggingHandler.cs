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
		_logger.LogInformation ("Sending request to {requestUrl}", request.RequestUri);
		var stopwatch = Stopwatch.StartNew ();

		HttpResponseMessage response = await base.SendAsync (request, cancellationToken);

		stopwatch.Stop ();
		_logger.LogInformation (
			"Received response from {requestUrl} in {elapsedMilliseconds} milliseconds.",
			request.RequestUri,
			stopwatch.ElapsedMilliseconds);

		return response;
	}
}