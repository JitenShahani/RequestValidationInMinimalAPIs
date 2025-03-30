namespace RequestValidationInMinimalAPIs.Services;

public class ServiceValidator (LoggingHandler loggingHandler, Database database)
{
	private readonly LoggingHandler _loggingHandler = loggingHandler;
	private readonly Database _database = database;
}