namespace DMS.Models
{
    /// <summary>
    /// Default error model
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        public ErrorModel[] Details { get; set; }
    }
}
