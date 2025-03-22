namespace RequestValidationInMinimalAPIs.DTO;

public record UpdatePostRequest (Guid Id, string Title, string Content);