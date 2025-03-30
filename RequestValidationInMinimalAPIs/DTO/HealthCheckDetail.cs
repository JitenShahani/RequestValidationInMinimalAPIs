namespace RequestValidationInMinimalAPIs.DTO;

public class HealthCheckDetail
{
	public required string Name { get; init; }
	public required string Status { get; init; }
	public required string Description { get; init; }
}