# Request Validation in Minimal APIs [![Target](https://img.shields.io/static/v1?label=Target&message=net9.0&color=512bd4&logo=.net&style=flat-square)](https://dotnet.microsoft.com/en-us/) [![Language](https://img.shields.io/badge/C%23-239120?label=&logo=c-sharp&style=flat-square)](https://dotnet.microsoft.com/en-us/languages/csharp/) [![Language](https://img.shields.io/badge/CSS-239120?label=&logo=css3&style=flat-square)]()

Request Validation in Minimal APIs is a .NET 9 application that demonstrates the use of minimal APIs in ASP.NET Core. This project includes various features such as global exception handling, CORS, HTTPS redirection, health checks, OpenAPI documentation, and more. It also showcases the use of middleware for handling exceptions and anti-forgery tokens, as well as the integration of Swagger UI with custom styles and dark mode support.

[![FluentValidation](https://img.shields.io/nuget/dt/FluentValidation.svg?label=FluentValidation&style=flat-square&logo=Fluent__Validation)](https://www.nuget.org/packages/FluentValidation/)
[![FluentValidation.DependencyInjection](https://img.shields.io/nuget/dt/FluentValidation.DependencyInjectionExtensions.svg?label=FluentValidation.DependencyInjectionExtensions&style=flat-square&logo=Fluent__Validation)](https://www.nuget.org/packages/FluentValidation.DependencyInjectionExtensions/)
[![OpenApi](https://img.shields.io/nuget/dt/Microsoft.AspNetCore.OpenApi.svg?label=Microsoft.AspNetCore.OpenApi&style=flat-square&logo=Microsoft)](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/)
[![HybridCaching](https://img.shields.io/nuget/dt/Microsoft.Extensions.Caching.Hybrid.svg?label=Microsoft.Extensions.Caching.Hybrid&style=flat-square&logo=Microsoft)](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Hybrid/)
[![HealthChecks](https://img.shields.io/nuget/dt/Microsoft.Extensions.Diagnostics.HealthChecks.svg?label=Microsoft.Extensions.Diagnostics.HealthChecks&style=flat-square&logo=Microsoft)](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks/)
[![SwaggerUI](https://img.shields.io/nuget/dt/Swashbuckle.AspNetCore.SwaggerUI.svg?label=Swashbuckle.AspNetCore.SwaggerUI&style=flat-square&logo=Swagger)](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerUI/)

## Features

- **Global Exception Handling**: Middleware to handle unhandled exceptions and return a standardized error response.
- **Static Assets**: Serve static assets like CSS, JS, and images.
- **Status Code Pages**: Provide better error responses with status code pages.
- **CORS**: Enable Cross-Origin Resource Sharing (CORS) for secure API access.
- **HTTPS Redirection**: Redirect HTTP requests to HTTPS in non-development environments.
- **Health Checks**: Implement health checks to monitor the application's health status.
- **OpenAPI Documentation**: Generate OpenAPI documentation for API endpoints.
- **Swagger UI**: Integrate Swagger UI with custom styles and dark mode support.
- **Rate Limiting**: Enable rate limiting to control the number of requests.
- **Anti-Forgery Tokens**: Use anti-forgery tokens to protect against CSRF attacks.
- **Hybrid Cache**: Use Hybrid Cache to cache data and improve performance.
- **Minimal APIs**: Demonstrate the use of minimal APIs for creating HTTP endpoints.

## Blog Post Endpoints

### Get all blog posts
- **Endpoint**: `GET /posts`
- **Description**: This endpoint returns all the blog posts from the database. The data will be cached using Hybrid Cache, and the response header `X-Data-Source` will indicate whether the data is coming from the Database or Cache.
- **Responses**:
  - `200 OK`: Returns a list of blog posts.
  - `204 No Content`: No blog posts found.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

### Get a blog post by Id
- **Endpoint**: `GET /posts/{id:Guid}`
- **Description**: This endpoint returns a single blog post based on the blog post Id you provide. The data will be cached using Hybrid Cache, and the response header `X-Data-Source` will indicate whether the data is coming from the Database or Cache.
- **Responses**:
  - `200 OK`: Returns the blog post.
  - `404 Not Found`: Blog post not found.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

### Create a new blog post
- **Endpoint**: `POST /posts`
- **Description**: This endpoint creates a new blog post based on the values you provide.
- **Request Body**:
  - `application/json`: A JSON object containing the title and content of the blog post.
- **Responses**:
  - `201 Created`: Returns the Id of the created blog post.
  - `400 Bad Request`: Invalid request data or missing anti-forgery token.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

### Update an existing blog post
- **Endpoint**: `PUT /posts`
- **Description**: This endpoint updates an existing blog post based on the values you provide.
- **Request Body**:
  - `application/json`: A JSON object containing the Id, title, and content of the blog post.
- **Responses**:
  - `200 OK`: Blog post updated successfully.
  - `404 Not Found`: Blog post not found.
  - `400 Bad Request`: Invalid request data or missing anti-forgery token.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

### Delete an existing blog post
- **Endpoint**: `DELETE /posts/{id:Guid}`
- **Description**: This endpoint deletes an existing blog post based on the blog post Id you provide.
- **Responses**:
  - `200 OK`: Blog post deleted successfully.
  - `404 Not Found`: Blog post not found.
  - `400 Bad Request`: Missing anti-forgery token.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

## Health Check Endpoint

### Get the health status of the application and the database
- **Endpoint**: `GET /health`
- **Description**: This endpoint returns the health status of the application along with it's database.
- **Responses**:
  - `200 OK`: Returns the health status of the application.
  - `204 No Content`: The named HttpClient service is missing, the endpoint response is not 200 OK, or the health report is null.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

## Exception Endpoint

### Trigger an unhandled exception
- **Endpoint**: `GET /exception`
- **Description**: This endpoint triggers an unhandled exception to demonstrate global exception handling.
- **Responses**:
  - `200 OK`: Returns the argument value if the Name parameter is not null.
  - `500 Internal Server Error`: Returns a standardized error response with details of the exception. All unhandled exception(s) are caught by the middleware.

## Anti-Forgery Token Endpoint

### Get Anti-Forgery Token
- **Endpoint**: `GET /AFT`
- **Description**: This endpoint returns the Anti-Forgery Token. The token is required for POST, PUT, and DELETE requests to prevent Cross-Site Request Forgery (CSRF) attacks. This endpoint will not be visible in the Swagger UI.
- **Responses**:
  - `200 OK`: Returns the Anti-Forgery Token.
  - `500 Internal Server Error`: Exception occurs. All unhandled exception(s) are caught by the middleware.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Compatible IDE
  - [![IDE](https://img.shields.io/badge/Visual_Studio-5C2D91?label=IDE&logo=visual%20studio&style=flat-square)](https://visualstudio.microsoft.com/)
  - [![IDE](https://img.shields.io/badge/Visual_Studio_Code-0078D4?label=Editor&logo=visual%20studio%20code&style=flat-square)](https://code.visualstudio.com/)
  - [![IDE](https://img.shields.io/badge/Rider-000000?label=IDE&logo=Rider&style=flat-square)](https://www.jetbrains.com/rider/)

### Installation

1. Clone the repository:
```
      git clone https://github.com/JitenShahani/RequestValidationInMinimalAPIs.git
      cd RequestValidationInMinimalAPIs
```

2. Build the project:
```
    dotnet build
```

3. Run the application:
```
    dotnet run --project .\RequestValidationInMinimalAPIs\RequestValidationInMinimalAPIs.csproj
```

Or , use the `watch` command to automatically restart the application when changes are detected:

```
    dotnet watch --project .\RequestValidationInMinimalAPIs\RequestValidationInMinimalAPIs.csproj
```

4. Open your browser and navigate to `https://localhost:7036/swagger/index.html` to view the Swagger UI.

## Examples

### Global Exception Handling
To see global exception handling in action, you can trigger an unhandled exception by accessing `/exception` endpoint that throws an ArgumentNull exception. The middleware will catch the exception and return a standardized error response.

### CORS
To test CORS, try making a request to the API from a different origin. The CORS policy will allow or deny the request based on the configured origins.

### HTTPS Redirection
To test HTTPS redirection, try accessing the application using HTTP. The application will automatically redirect the request to HTTPS provided the environment is not development.

### Health Checks
To check the health status of the application, navigate to the `/health` endpoint. The application will return the health status of various components.

### OpenAPI Documentation
To view the OpenAPI documentation, navigate to the `/swagger` endpoint. The Swagger UI will display the API documentation with custom styles and dark mode support.

### Rate Limiting
To test rate limiting, make multiple requests to the API within a short period. The rate limiter will enforce the configured limits and return a `429 Too Many Requests` response if the limit is exceeded.

### Anti-Forgery Tokens
To test anti-forgery tokens, try making a POST, PUT, or DELETE request without including the anti-forgery token in the header. The request will be rejected to protect against Cross-Site Request Forgery (CSRF) attacks.

### Hybrid Cache
To see Hybrid Cache in action, navigate to the `/posts` or `/posts/{id:Guid}` endpoints. The data will be cached for 5 minutes, and the response header `X-Data-Source` will indicate whether the data is coming from the Database or Cache.

### Minimal APIs
To see minimal APIs in action, navigate to the various endpoints defined in the application. The minimal APIs provide a simple and efficient way to create HTTP endpoints.

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE.txt) file for more details.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue to discuss any changes or improvements.