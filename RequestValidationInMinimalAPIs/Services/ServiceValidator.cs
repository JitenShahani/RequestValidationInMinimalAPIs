namespace RequestValidationInMinimalAPIs.Services;

public class ServiceValidator
{
	private readonly LoggingHandler _loggingHandler;
	private readonly Database _database;

	public ServiceValidator (LoggingHandler loggingHandler, Database database) =>
		(_loggingHandler, _database) = (loggingHandler, database);
}