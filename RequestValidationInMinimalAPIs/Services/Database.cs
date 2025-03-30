namespace RequestValidationInMinimalAPIs.Services;

public class Database
{
	private readonly List<Post> posts =
	[
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Hello, World!",
			Content = "This is a sample post. Welcome to my blog on all things Asp Net Core."
		},
		new Post()
		{
			Id = Guid.CreateVersion7(),
			Title = "OpenAPI document generation in .NET 9",
			Content = "ASP.NET Core in .NET 9 streamlines the process of creating OpenAPI documents for your API endpoints with new built-in support for OpenAPI document generation. This new feature is designed to simplify the development workflow and improve the integration of OpenAPI definitions in your ASP.NET applications. And OpenAPI’s broad adoption has fostered a rich ecosystem of tools and services that can help you build, test, and document your APIs more effectively. Some examples are Swagger UI, the Kiota client library generator, and Redoc, but there are many, many more."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Getting Started with Minimal APIs in .NET 6",
			Content = "Minimal APIs in .NET 6 provide a simple way to create HTTP APIs with minimal dependencies. This post will guide you through the basics of setting up a minimal API project and creating your first endpoint."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Understanding Dependency Injection in ASP.NET Core",
			Content = "Dependency Injection (DI) is a fundamental concept in ASP.NET Core. This post explains what DI is, why it's important, and how to use it effectively in your ASP.NET Core applications."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Exploring the New Features in C# 10",
			Content = "C# 10 introduces several new features and enhancements that make the language more powerful and expressive. In this post, we'll explore some of the most exciting new features in C# 10 and how you can use them in your projects."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Building Real-Time Applications with SignalR",
			Content = "SignalR is a library for ASP.NET Core that makes it easy to add real-time web functionality to applications. This post will show you how to get started with SignalR and build a real-time chat application."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Securing Your ASP.NET Core Applications",
			Content = "Security is a critical aspect of any web application. This post covers various security practices and features in ASP.NET Core, including authentication, authorization, and data protection."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Performance Tuning in ASP.NET Core",
			Content = "Performance is key to the success of any web application. This post provides tips and techniques for optimizing the performance of your ASP.NET Core applications."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Deploying ASP.NET Core Applications to Azure",
			Content = "Azure provides a range of services for hosting and managing ASP.NET Core applications. This post guides you through the process of deploying your ASP.NET Core application to Azure."
		},
		new Post
		{
			Id = Guid.CreateVersion7(),
			Title = "Unit Testing in ASP.NET Core",
			Content = "Unit testing is an essential part of the development process. This post explains how to write and run unit tests for your ASP.NET Core applications using xUnit and Moq."
		}
	];

	public List<Post> Posts => posts;
}