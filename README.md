# Request Validation in Minimal APIs

## Overview

Request Validation in Minimal APIs is a .NET 9 application that demonstrates the use of minimal APIs in ASP.NET Core. This project includes various features such as global exception handling, CORS, HTTPS redirection, health checks, OpenAPI documentation, and more. It also showcases the use of middleware for handling exceptions and anti-forgery tokens, as well as the integration of Swagger UI with custom styles and dark mode support.

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
- **Minimal APIs**: Demonstrate the use of minimal APIs for creating HTTP endpoints.

## Technologies Used

- **.NET 9**
- **ASP.NET Core**
- **Swagger UI**

## Getting Started

### Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or any other compatible IDE

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
To see global exception handling in action, you can trigger an unhandled exception by accessing an endpoint that throws an exception. The middleware will catch the exception and return a standardized error response.

### CORS
To test CORS, try making a request to the API from a different origin. The CORS policy will allow or deny the request based on the configured origins.

### HTTPS Redirection
To test HTTPS redirection, try accessing the application using HTTP. The application will automatically redirect the request to HTTPS.

### Health Checks
To check the health status of the application, navigate to the `/health` endpoint. The application will return the health status of various components.

### OpenAPI Documentation
To view the OpenAPI documentation, navigate to the `/swagger` endpoint. The Swagger UI will display the API documentation with custom styles and dark mode support.

### Rate Limiting
To test rate limiting, make multiple requests to the API within a short period. The rate limiter will enforce the configured limits and return a `429 Too Many Requests` response if the limit is exceeded.

### Anti-Forgery Tokens
To test anti-forgery tokens, try making a POST request without including the anti-forgery token in the headers. The request will be rejected to protect against CSRF attacks.

### Minimal APIs
To see minimal APIs in action, navigate to the various endpoints defined in the application. The minimal APIs provide a simple and efficient way to create HTTP endpoints.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue to discuss any changes or improvements.