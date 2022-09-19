using DMS.Components.DeadManSwitch;
using DMS.DataProviders.Login;
using DMS.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DMS.Components.Login
{
    public class LoginComponent : ILoginComponent
    {
        // TODO: make this a config option for when user creates account
        private readonly ILogger logger;
        private readonly ILoginRepository loginRepository;
        private readonly IDeadManSwitchComponent deadManSwitch;

        public LoginComponent(ILogger<LoginComponent> logger, ILoginRepository loginRepository, IDeadManSwitchComponent deadManSwitch)
        {
            this.logger = logger;
            this.loginRepository = loginRepository;
            this.deadManSwitch = deadManSwitch;
        }

        /// <summary>
        /// Daily login for the user
        /// When the user logs in, the user's login date is updated to the current time.
        /// </summary>
        /// <returns></returns>
        public async Task<LoginResponseModel> Login(Guid accountId, CancellationToken cancellationToken)
        {
            /*
             * 1. Get the user's login date
             * 2. If the user's login data is not found, create a new user
             * 3. If the users's most recent login is before now minus the threshold, then flip the dead man switch
             * 4. If the user's most recent login is within the threshold, update the most recent login to be now
             */
            DateTime currentTime = DateTime.UtcNow;
            LoginKey key = new()
            {
                AccountId = accountId
            };
            LoginResponseModel? existingLogin = await loginRepository.GetLoginModel(key, cancellationToken);
            if (existingLogin == null)
            {
                // TODO: return a 404 error code
                throw new Exception("User not found");   
            }
            // If the users's most recent login is before now minus the threshold, then flip the dead man switch 
            else if (DateTime.Compare(existingLogin.LastModifiedAt, currentTime - existingLogin.DeadManSwitchInterval) < 0)
            {
                string message = "You did not login in time. Flipping the switch";
                logger.LogWarning(message);

                await this.deadManSwitch.Send(cancellationToken);
                // logging into an account that is past the deadline should result in what status code?
                throw new Exception(message);
            }

            // If the user's most recent login is within the threshold, update the most recent login to be now
            existingLogin.LastModifiedAt = currentTime;

            return await loginRepository.AddOrUpdateLogin(existingLogin, cancellationToken);
        }

        public async Task<LoginResponseModel> SignUp(Guid accountId, SignUpRequestModel signUp, CancellationToken cancellationToken)
        {
            LoginKey key = new()
            {
                AccountId = accountId
            };
            LoginResponseModel? existingLogin = await loginRepository.GetLoginModel(key, cancellationToken);
            if (existingLogin != null)
            {
                // TODO: return a 409 error code
                throw new Exception("User already exists");
            }

            DateTime currentTime = DateTime.UtcNow;
            return await loginRepository.AddOrUpdateLogin(new()
            {
                AccountId = accountId,
                CreatedAt = currentTime,
                DeadManSwitchInterval = signUp.DeadManSwitchInterval,
                LastModifiedAt = currentTime,
            }, cancellationToken);
        }
    }
}
