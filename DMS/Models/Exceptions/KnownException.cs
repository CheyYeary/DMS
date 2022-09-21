namespace DMS.Models.Exceptions
{
    public class KnownException : Exception
    {
        public ErrorCategory Category { get; }

        public ServiceErrorCode Code { get; }

        public string? Details { get; }

        public KnownException() : base("Generic Exception")
        {
        }

        public KnownException(string message) : base(message)
        {
        }

        public KnownException(
            ErrorCategory category,
            ServiceErrorCode code,
            string message,
            string details = null,
            Exception innerException = null)
            : base(message, innerException)
        {
            this.Category = category;
            this.Code = code;
            this.Details = details;
        }
    }
}
