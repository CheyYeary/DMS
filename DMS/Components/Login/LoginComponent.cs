using DMS.DataProviders.DataFactory;
using DMS.DataProviders.Login;
using DMS.Models;
using DMS.Models.Exceptions;

namespace DMS.Components.Login
{
    public class LoginComponent : ILoginComponent
    {
        // TODO: make this a config option for when user creates account
        private readonly ILogger logger;
        private readonly ILoginRepository loginRepository;
        private readonly IDataFactoryService dataFactory;

        public LoginComponent(ILogger<LoginComponent> logger, ILoginRepository loginRepository, IDataFactoryService dataFactory)
        {
            this.logger = logger;
            this.loginRepository = loginRepository;
            this.dataFactory = dataFactory;
        }

        /// <inheritdoc/>
        public async Task<LoginResponseModel> Login(Guid accountId, CancellationToken cancellationToken)
        {
            /*
             * 1. Get the user's login date
             * 2. If the user's login data is not found, create a new user
             * 3. Update the most recent login to be now
             */
            DateTime currentTime = DateTime.UtcNow;
            LoginKey key = new()
            {
                AccountId = accountId
            };

            LoginResponseModel? existingLogin = await loginRepository.GetLoginModel(key, cancellationToken);
            if (existingLogin == null)
            {
                throw new KnownException(ErrorCategory.ResourceNotFound, ServiceErrorCode.User_NotFound, "User not found");   
            }

            /*
             * 1. Update the most recent login to be now
             * 2. Update the trigger with start time set to now
             */
            existingLogin.LastModifiedAt = currentTime;
            existingLogin.Recurrence.StartTime = currentTime;

            string triggerName = GetTriggerName(accountId);
            await this.dataFactory.GetTrigger(triggerName, cancellationToken);
            await this.dataFactory.CreateTrigger(accountId, triggerName, existingLogin.Recurrence, cancellationToken);

            return await loginRepository.AddOrUpdateLogin(existingLogin, cancellationToken);
        }

        /// <inheritdoc/>
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
                throw new KnownException(ErrorCategory.Conflict, ServiceErrorCode.User_Exists, "User already exists");
            }

            await this.dataFactory.CreatePipeline(accountId, cancellationToken);
            string triggerName = GetTriggerName(accountId);
            await this.dataFactory.CreateTrigger(accountId, triggerName, signUp.Recurrence, cancellationToken);

            DateTime currentTime = DateTime.UtcNow;
            return await loginRepository.AddOrUpdateLogin(new()
            {
                AccountId = accountId,
                CreatedAt = currentTime,
                Recurrence = signUp.Recurrence,
                LastModifiedAt = currentTime,
            }, cancellationToken);
        }

        private static string GetTriggerName(Guid accountId) => $"{accountId}_DeadManSwitchTrigger";
    }
}
