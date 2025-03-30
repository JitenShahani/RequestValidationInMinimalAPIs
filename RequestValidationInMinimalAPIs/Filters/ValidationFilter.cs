namespace RequestValidationInMinimalAPIs.Filters;

public class ValidationFilter<TRequest> : IEndpointFilter
{
	private readonly IValidator<TRequest> _validator;

	public ValidationFilter (IValidator<TRequest> validator) =>
		_validator = validator;

	public async ValueTask<object?> InvokeAsync (
		EndpointFilterInvocationContext context,
		EndpointFilterDelegate next)
	{
		var request = context.Arguments.OfType<TRequest> ().First ();

		var result = await _validator.ValidateAsync (request, context.HttpContext.RequestAborted);

		ProblemDetailsFactory problemDetailsFactory =
			context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory> ();

		ProblemDetails problemDetails = problemDetailsFactory
			.CreateProblemDetails (context.HttpContext, StatusCodes.Status400BadRequest, "Validation error(s)");

		problemDetails.Extensions["Errors"] = result.ToDictionary ();

		if (!result.IsValid)
			return TypedResults.Problem (problemDetails);

		return await next (context);
	}
}