using DMS.Models;

namespace DMS.DataProviders.Login
{
    public interface ILoginRepository
    {
        Task<LoginResponseModel?> GetLoginModel(LoginKey key, CancellationToken cancellationToken);

        Task<LoginResponseModel> AddOrUpdateLogin(LoginResponseModel existingLogin, CancellationToken cancellationToken);

    }
}
