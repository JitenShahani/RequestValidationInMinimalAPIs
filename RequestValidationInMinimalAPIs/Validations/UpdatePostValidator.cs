namespace RequestValidationInMinimalAPIs.Validations;

public class UpdatePostValidator : AbstractValidator<UpdatePostRequest>
{
	public UpdatePostValidator ()
	{
		RuleFor (p => p.Id)
			.NotEmpty ()
			.Must (id => IsGuidVersion7 (id))
				.WithMessage ("Id must be a valid Version 7 Guid"); ;

		RuleFor (p => p.Title)
			.NotEmpty ()
			.MinimumLength (5)
			.Must (title => title != "string")
				.WithMessage ("Title cannot be 'string'"); ;

		RuleFor (p => p.Content)
			.NotEmpty ()
			.MinimumLength (15)
			.Must (content => content != "string")
				.WithMessage ("Content cannot be 'string'"); ;
	}

	private static bool IsGuidVersion7 (Guid guid)
	{
		var version = (guid.ToByteArray ()[7] >> 4);
		return version == 7;
	}
}