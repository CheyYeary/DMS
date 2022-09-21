using DMS.Models;

namespace DMS.Components.Login
{
    public interface ILoginComponent
    {
        /// <summary>
        /// When the user logs in, the user's login date is updated to the current time.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LoginResponseModel> Login(Guid accountId, CancellationToken cancellationToken);

        /// <summary>
        /// Create a user account and upload the user's data to the blob storage
        /// Create the trigger used to schedule the deadman switch
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="signup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LoginResponseModel> SignUp(Guid accountId, SignUpRequestModel signup, CancellationToken cancellationToken);

    }
}
