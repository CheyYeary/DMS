namespace DMS.Models.Exceptions
{
    public enum ServiceErrorCode
    {
        #region General Exception

        /// <summary>
        /// The service threw an exception which is a result of a code bug.
        /// </summary>
        UnknownException = 1000,

        /// <summary>
        /// A missing required field.
        /// </summary>
        MissingField,

        /// <summary>
        /// An invalid value for a field.
        /// </summary>
        InvalidField,

        /// <summary>
        /// Problems accessing persistence.
        /// </summary>
        DatabaseException,

        /// <summary>
        /// Missing client certificate.
        /// </summary>
        MissingClientCertificate,

        /// <summary>
        /// Invalid client certificate.
        /// </summary>
        InvalidClientCertificate,

        /// <summary>
        /// The operation not allowed.
        /// </summary>
        OperationNotAllowed,

        /// <summary>
        /// Invalid type of model
        /// </summary>
        InvalidType,

        /// <summary>
        /// Problems accessing storage.
        /// </summary>
        StorageException,

        #endregion
        
        /// <summary>
        /// The user was not found.
        /// </summary>
        User_NotFound = 1100,

        /// <summary>
        /// The user was not found.
        /// </summary>
        User_Exists = 1101,

        File_NotFound = 1102,
    }
}
