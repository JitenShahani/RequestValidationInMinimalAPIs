namespace RequestValidationInMinimalAPIs.Validations;

public class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
	public CreatePostValidator ()
	{
		RuleFor (p => p.Title)
			.NotEmpty ()
			.MinimumLength (5)
			.Must (title => title != "string")
				.WithMessage ("Title cannot be 'string'");

		RuleFor (p => p.Content)
			.NotEmpty ()
			.MinimumLength (15)
			.Must (content => content != "string")
				.WithMessage ("Content cannot be 'string'");
	}
}