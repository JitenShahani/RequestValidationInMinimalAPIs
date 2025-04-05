namespace RequestValidationInMinimalAPIs.Endpoints;

public class PostEndpoints
{
	private readonly string _headerName;
	private readonly string _getAllPostsKey;

	public PostEndpoints (IConfiguration configuration)
	{
		_headerName = configuration["AntiForgeryToken:HeaderName"]!;
		_getAllPostsKey = configuration["CacheKeys:GetPostsKey"]!;
	}

	public void MapPostEndpoints (IEndpointRouteBuilder app)
	{
		var postEndpoints = app.MapGroup ("/posts")
			.WithOpenApi ()
			.RequireCors ()
			.RequireRateLimiting ("Concurrency")
			.RequireRateLimiting ("FixedWindow")
			.WithTags ("Posts")
			.ProducesProblem (StatusCodes.Status500InternalServerError);		// Global Exception Handler will handle this

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

	internal async Task<Results<Ok<List<Post>>, NoContent>> GetPosts (
		[FromServices] HybridCache hybridCache,
		[FromServices] Database database,
		[FromServices] IAntiforgery antiforgery,
		HttpContext httpContext,
		CancellationToken cancellationToken)
	{
		bool isFromCache = true;

		// Fetch posts from cache or database
		List<Post> posts = await hybridCache
			.GetOrCreateAsync (
				_getAllPostsKey,
				async token =>
				{
					isFromCache = false;
					return await Task.FromResult (database.Posts.ToList ());
				},
				cancellationToken: cancellationToken);

		// Set response header based on data source
		httpContext.Response.Headers.Append ("X-Data-Source", isFromCache ? "Cache" : "Database");

		// Generate the anti-forgery token
		httpContext.Response.Headers.Append (_headerName, antiforgery.GetAndStoreTokens (httpContext).RequestToken);

		// Return the appropriate result based on the posts count
		return posts.Count > 0
			? TypedResults.Ok (posts)
			: TypedResults.NoContent ();
	}

	internal async Task<Results<Ok<Post>, NotFound>> GetPostById (
		[FromServices] HybridCache hybridCache,
		[FromRoute] Guid id,
		[FromServices] Database database,
		HttpContext httpContext,
		CancellationToken cancellationToken)
	{
		bool isFromCache = true;

		List<Post> posts = await hybridCache
			.GetOrCreateAsync (
				_getAllPostsKey,
				async token =>
				{
					isFromCache = false;
					var post = database.Posts.FirstOrDefault (p => p.Id == id);
					return await Task.FromResult (
						post is not null
							? new List<Post> { post }
							: []);
				},
				cancellationToken: cancellationToken);

		var post = isFromCache
			? posts.FirstOrDefault (p => p.Id == id)
			: posts.Count > 0
				? posts.First ()
				: null;

		// If single post is retrieved from the database, I would rather remove the cache as the cache key is same between GetPosts and GetPostById. Otherwise, I will fetch the single matching post from the cache.
		if (!isFromCache)
			await hybridCache.RemoveAsync (_getAllPostsKey, cancellationToken: httpContext.RequestAborted);

		// Set response header based on data source
		if (post is not null)
			httpContext.Response.Headers.Append ("X-Data-Source", isFromCache ? "Cache" : "Database");

		return post is not null
			? TypedResults.Ok (post)
			: TypedResults.NotFound ();
	}

	[ValidateAntiForgeryToken]
	internal async Task<Created<Guid>> CreatePost (
		[FromBody] CreatePostRequest request,
		[FromServices] HybridCache hybridCache,
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

		// Clear the cache to return updated values from the database on next request.
		await hybridCache.RemoveAsync (_getAllPostsKey, cancellationToken: httpContext.RequestAborted);

		var uri = new Uri ($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}/{post.Id}");

		return TypedResults.Created (uri, post.Id);
	}

	[ValidateAntiForgeryToken]
	internal async Task<Results<Ok, NotFound>> UpdatePost (
		[FromBody] UpdatePostRequest request,
		[FromServices] HybridCache hybridCache,
		[FromServices] Database database,
		[FromHeader (Name = "X-AFT-Value")] string token,
		HttpContext httpContext)
	{
		var post = database.Posts.FirstOrDefault (x => x.Id == request.Id);

		if (post is null)
			return TypedResults.NotFound ();

		post.Title = request.Title.Trim ();
		post.Content = request.Content.Trim ();

		// Clear the cache to return updated values from the database on next request.
		await hybridCache.RemoveAsync (_getAllPostsKey, cancellationToken: httpContext.RequestAborted);

		return TypedResults.Ok ();
	}

	[ValidateAntiForgeryToken]
	internal async Task<Results<Ok, NotFound>> DeletePost (
		[FromRoute] Guid id,
		[FromServices] HybridCache hybridCache,
		[FromServices] Database database,
		[FromHeader (Name = "X-AFT-Value")] string token,
		HttpContext httpContext)
	{
		var post = database.Posts.FirstOrDefault (p => p.Id == id);

		bool postRemoved = false;

		if (post is not null)
			postRemoved = database.Posts.Remove (post);

		// Clear the cache to return updated values from the database on next request.
		if (postRemoved)
			await hybridCache.RemoveAsync(_getAllPostsKey, cancellationToken: httpContext.RequestAborted);

		return postRemoved
			? TypedResults.Ok ()
			: TypedResults.NotFound ();
	}
}