namespace RequestValidationInMinimalAPIs.DTO;

public class HealthCheck
{
	public required string ApiStatus { get; init; }
	public IEnumerable<HealthCheckDetail>? Checks { get; init; }
}