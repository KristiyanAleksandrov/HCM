namespace Auth.Application.Errors
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string message, int status) : base(message) =>
            StatusCode = status;

        public int StatusCode { get; }
    }

    public sealed class ConflictException : ApiException { public ConflictException(string m) : base(m, 409) { } }
    public sealed class NotFoundException : ApiException { public NotFoundException(string m) : base(m, 404) { } }
    public sealed class BadRequestException : ApiException { public BadRequestException(string message) : base(message, 400) { } }
    public sealed class UnauthorizedException : ApiException { public UnauthorizedException() : base("Invalid credentials", 401) { } }
}
