namespace RequestValidationInMinimalAPIs.Filters;

public static class ValidationExtension
{
	public static RouteHandlerBuilder WithRequestValidation<TRequest> (this RouteHandlerBuilder builder)
	{
		return builder
			.AddEndpointFilter<ValidationFilter<TRequest>> ()
			.ProducesValidationProblem ();
	}
}