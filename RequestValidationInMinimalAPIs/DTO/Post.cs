namespace RequestValidationInMinimalAPIs.DTO;

public class Post
{
	public Guid Id { get; init; } = Guid.CreateVersion7 ();
	public required string Title { get; set; }
	public required string Content { get; set; }
}