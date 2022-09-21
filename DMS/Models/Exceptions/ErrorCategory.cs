namespace DMS.Models.Exceptions
{
    public enum ErrorCategory
    {
        /// <summary>
        /// An input error.
        /// </summary>
        InputError,

        /// <summary>
        /// A problem in the service code which is unrelated to caller's inputs.
        /// </summary>
        ServiceError,

        /// <summary>
        /// A dependency's problem.
        /// </summary>
        DownStreamError,

        /// <summary>
        /// The requested resource was not found.
        /// </summary>
        ResourceNotFound,

        /// <summary>
        /// The service can't access a back end dependency.
        /// </summary>
        BackEndDependencyInaccessible,

        /// <summary>
        /// The requested operation is not allowed.
        /// </summary>
        Forbidden,

        /// <summary>
        /// An operation is being called too frequently. Callers need to back off for sometime.
        /// </summary>
        TooManyRequests,

        /// <summary>
        /// The operation cannot be completed because it results in a conflict with already existing rule/resource.
        /// </summary>
        Conflict,

        /// <summary>
        /// Authentication problem.
        /// </summary>
        AuthenticationError,
    }
}
