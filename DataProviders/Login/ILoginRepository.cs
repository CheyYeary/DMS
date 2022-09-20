using DMS.Models;
namespace DMS.DataProviders.Login
{
    public interface ILoginRepository
    {
        /// <summary>
        /// Get the user login info
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LoginResponseModel> GetLoginModel(LoginKey key, CancellationToken cancellationToken);

        /// <summary>
        /// Try to create the container if it does not exist
        /// Then upload the blob to the container
        /// </summary>
        /// <param name="existingLogin"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LoginResponseModel> AddOrUpdateLogin(LoginResponseModel existingLogin, CancellationToken cancellationToken);

    }
}
