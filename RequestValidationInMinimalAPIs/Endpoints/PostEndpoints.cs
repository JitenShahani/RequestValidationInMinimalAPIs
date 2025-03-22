namespace RequestValidationInMinimalAPIs.Endpoints;

public class PostEndpoints
{
	private readonly string _headerName;

	public PostEndpoints (IConfiguration configuration) =>
		_headerName = configuration["AntiForgeryToken:HeaderName"]!;

	public void MapPostEndpoints (IEndpointRouteBuilder app)
	{
		var postEndpoints = app.MapGroup ("/posts")
			.WithOpenApi ()
			.RequireCors ()
			.RequireRateLimiting ("Concurrency")
			.RequireRateLimiting ("FixedWindow")
			.WithTags ("Posts")
			.ProducesProblem(StatusCodes.Status500InternalServerError);

		postEndpoints.MapGet ("/", GetPosts)
			.WithSummary ("Get all blog posts")
			.WithDescription ("This endpoint returns all the blog posts from the database.")
			.Produces<List<Post>> (StatusCodes.Status200OK)
			.Produces (StatusCodes.Status204NoContent);

		postEndpoints.MapGet ("/{id:Guid}", GetPostById)
			.WithSummary ("Get a blog post by Id")
			.WithDescription ("This endpoint returns a single blog post based on the blog post Id you will provide below.")
			.Produces<Post> (StatusCodes.Status200OK)
			.Produces (StatusCodes.Status404NotFound);

		postEndpoints.MapPost ("/", CreatePost)
			.WithSummary ("Create a new blog post")
			.WithDescription ("This endpoint creates a new blog post based on the values you provide below.")
			.Accepts<CreatePostRequest> ("application/json")
			.Produces<Guid> (StatusCodes.Status201Created)
			.Produces (StatusCodes.Status400BadRequest) // Endpoint doesn't return 400. Anti Forgery middleware does, on invalid token.
			.WithRequestValidation<CreatePostRequest> ();

		postEndpoints.MapPut ("/", UpdatePost)
			.WithSummary ("Update an existing blog post")
			.WithDescription ("This endpoint updates an existing blog post based on the values you provide below.")
			.Accepts<UpdatePostRequest> ("application/json")
			.Produces (StatusCodes.Status200OK)
			.Produces (StatusCodes.Status404NotFound)
			.Produces (StatusCodes.Status400BadRequest) // Endpoint doesn't return 400. Anti Forgery middleware does, on invalid token.
			.WithRequestValidation<UpdatePostRequest> ();

		postEndpoints.MapDelete ("/{id:Guid}", DeletePost)
			.WithSummary ("Delete an existing blog post")
			.WithDescription ("This endpoint deletes an existing blog post based on the blog post Id you will provide below.")
			.Produces (StatusCodes.Status200OK)
			.Produces (StatusCodes.Status404NotFound)
			.Produces (StatusCodes.Status400BadRequest); // Endpoint doesn't return 400. Anti Forgery middleware does, on invalid token.
	}

	internal Results<Ok<List<Post>>, NoContent> GetPosts (
		[FromServices] Database database,
		[FromServices] IAntiforgery antiforgery,
		HttpContext httpContext)
	{
		var posts = database.Posts.ToList ();

		// Generate the anti-forgery token
		var token = antiforgery.GetAndStoreTokens (httpContext).RequestToken;
		httpContext.Response.Headers.Append (_headerName, token);

		return
		posts.Count > 0
		? TypedResults.Ok (posts)
		: TypedResults.NoContent ();
	}

	internal Results<Ok<Post>, NotFound> GetPostById ([FromRoute] Guid id, [FromServices] Database database)
	{
		var post = database.Posts.FirstOrDefault (p => p.Id == id);

		return post is not null
			? TypedResults.Ok (post)
			: TypedResults.NotFound ();
	}

	[ValidateAntiForgeryToken]
	internal Created<Guid> CreatePost (
		[FromBody] CreatePostRequest request,
		[FromServices] Database database,
		[FromHeader (Name = "X-AFT-Value")] string token,
		HttpContext httpContext)
	{
		var post = new Post ()
		{
			Title = request.Title.Trim (),
			Content = request.Content.Trim ()
		};

		database.Posts.Add (post);

		var uri = new Uri ($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}/{post.Id}");

		return TypedResults.Created (uri, post.Id);
	}

	[ValidateAntiForgeryToken]
	internal Results<Ok, NotFound> UpdatePost (
		[FromBody] UpdatePostRequest request,
		[FromServices] Database database,
		[FromHeader (Name = "X-AFT-Value")] string token,
		HttpContext httpContext)
	{
		var post = database.Posts.FirstOrDefault (x => x.Id == request.Id);

		if (post is null)
			return TypedResults.NotFound ();

		post.Title = request.Title.Trim ();
		post.Content = request.Content.Trim ();

		return TypedResults.Ok ();
	}

	[ValidateAntiForgeryToken]
	internal Results<Ok, NotFound> DeletePost (
		[FromRoute] Guid id,
		[FromServices] Database database,
		[FromHeader (Name = "X-AFT-Value")] string token,
		HttpContext httpContext)
	{
		var post = database.Posts.FirstOrDefault (p => p.Id == id);

		bool postRemoved = false;

		if (post is not null)
			postRemoved = database.Posts.Remove (post);

		return postRemoved
			? TypedResults.Ok ()
			: TypedResults.NotFound ();
	}
}