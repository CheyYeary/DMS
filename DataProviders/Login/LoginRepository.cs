using DMS.Models;

namespace DMS.DataProviders.Login
{
    public class LoginRepository : ILoginRepository
    {
        public Task<LoginResponseModel> AddOrUpdateLogin(LoginResponseModel existingLogin, CancellationToken cancellationToken)
        {
            // TODO: Implement this
            //mock data
            return Task.FromResult(existingLogin);
        }

        public Task<LoginResponseModel?> GetLoginModel(LoginKey key, CancellationToken cancellationToken)
        {
            // TODO: Query using key to check if it exists

            try
            {
                //mock data
                DateTime currentTime = DateTime.UtcNow - TimeSpan.FromDays(1);
                return Task.FromResult(new LoginResponseModel()
                {
                    AccountId = key.AccountId,
                    CreatedAt = currentTime,
                    DeadManSwitchInterval = TimeSpan.FromMinutes(1),
                    LastModifiedAt = currentTime
                });
            }
            catch (Exception ex) // TODO: catch exception only for not found
            {
                return Task.FromResult((LoginResponseModel?)null);
            }

            
        }
    }
}
