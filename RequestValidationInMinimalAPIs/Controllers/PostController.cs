/*
using Microsoft.AspNetCore.Cors;

namespace RequestValidationInMinimalAPIs.Controllers;

[ApiController]
[Route ("/api/[controller]")]
[Tags ("Posts - Controller Endpoints")]
[EnableCors]
[EnableRateLimiting ("Concurrency")]
[ProducesResponseType<ProblemDetails> (StatusCodes.Status500InternalServerError)]
public class PostController : ControllerBase
{
	private readonly string _headerName;
	private readonly Database _database;
	private readonly IAntiforgery _antiforgery;

	public PostController (IConfiguration configuration, Database database, IAntiforgery antiforgery)
	{
		_headerName = configuration["AntiForgeryToken:HeaderName"]!;
		_database = database;
		_antiforgery = antiforgery;
	}

	[HttpGet]
	[EndpointSummary ("Get all blog posts")]
	[EndpointDescription ("This endpoint returns all the blog posts from the database.")]
	[ProducesResponseType<List<Post>> (StatusCodes.Status200OK)]
	[ProducesResponseType (StatusCodes.Status204NoContent)]
	[EnableRateLimiting ("FixedWindow")]
	public ActionResult<List<Post>> GetPosts ()
	{
		var posts = _database.Posts.ToList ();

		// Generate the anti-forgery token
		var token = _antiforgery.GetAndStoreTokens (HttpContext).RequestToken;
		HttpContext.Response.Headers.Append (_headerName, token);

		return posts.Count > 0 ? Ok (posts) : NoContent ();
	}

	[HttpGet ("{id:Guid}")]
	[EndpointSummary ("Get a blog post by Id")]
	[EndpointDescription ("This endpoint returns a single blog post based on the blog post Id you will provide below.")]
	[ProducesResponseType<Post> (StatusCodes.Status200OK)]
	[ProducesResponseType (StatusCodes.Status404NotFound)]
	[EnableRateLimiting ("FixedWindow")]
	public ActionResult<Post> GetPostById (Guid id)
	{
		var post = _database.Posts.FirstOrDefault (p => p.Id == id);

		return post is not null ? Ok (post) : NotFound ();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[ServiceFilter<ValidationActionFilter<CreatePostRequest>> ()]
	[EndpointSummary ("Create a new blog post")]
	[EndpointDescription ("This endpoint creates a new blog post based on the values you provide below.")]
	[Consumes ("application/json")]
	[ProducesResponseType<Guid> (StatusCodes.Status201Created)]
	[ProducesResponseType (StatusCodes.Status400BadRequest)]
	[EnableRateLimiting ("FixedWindow")]
	public ActionResult<Guid> CreatePost ([FromBody] CreatePostRequest request, [FromHeader (Name = "X-AFT-Value")] string token)
	{
		var post = new Post
		{
			Title = request.Title.Trim (),
			Content = request.Content.Trim ()
		};

		_database.Posts.Add (post);

		var uri = new Uri ($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}/{post.Id}");

		return Created (uri, post.Id);
	}

	[HttpPut]
	[ValidateAntiForgeryToken]
	[ServiceFilter<ValidationActionFilter<UpdatePostRequest>> ()]
	[EndpointSummary ("Update an existing blog post")]
	[EndpointDescription ("This endpoint updates an existing blog post based on the values you provide below.")]
	[Consumes ("application/json")]
	[ProducesResponseType (StatusCodes.Status200OK)]
	[ProducesResponseType (StatusCodes.Status404NotFound)]
	[ProducesResponseType (StatusCodes.Status400BadRequest)]
	public ActionResult UpdatePost ([FromBody] UpdatePostRequest request, [FromHeader (Name = "X-AFT-Value")] string token)
	{
		var post = _database.Posts.FirstOrDefault (x => x.Id == request.Id);

		if (post is null)
			return NotFound ();

		post.Title = request.Title.Trim ();
		post.Content = request.Content.Trim ();

		return Ok ();
	}

	[HttpDelete ("{id:Guid}")]
	[ValidateAntiForgeryToken]
	[EndpointSummary ("Delete an existing blog post")]
	[EndpointDescription ("This endpoint deletes an existing blog post based on the blog post Id you will provide below.")]
	[ProducesResponseType (StatusCodes.Status200OK)]
	[ProducesResponseType (StatusCodes.Status404NotFound)]
	[ProducesResponseType (StatusCodes.Status400BadRequest)]
	public ActionResult DeletePost ([FromRoute] Guid id, [FromHeader (Name = "X-AFT-Value")] string token)
	{
		var post = _database.Posts.FirstOrDefault (p => p.Id == id);

		if (post is null)
			return NotFound ();

		_database.Posts.Remove (post);

		return Ok ();
	}
}
*/