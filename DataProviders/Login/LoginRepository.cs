using Azure.Storage.Blobs.Models;
using Azure;
using DMS.Models;

namespace DMS.DataProviders.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IBlobService blobService;
        private const string UserLoginPath = "login.json";

        public LoginRepository(IBlobService blobService)
        {
            this.blobService = blobService;
        }
        
        /// <inheritdoc/>
        public async Task<LoginResponseModel> AddOrUpdateLogin(LoginResponseModel existingLogin, CancellationToken cancellationToken)
        {
            try
            {
                await this.blobService.CreateContainer(existingLogin.AccountId.ToString());
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerAlreadyExists)
            {
                // skip creating container when it already exists during login flow
            }

            return await this.blobService.UploadObjectToBlob(UserLoginPath, existingLogin.AccountId.ToString(), existingLogin, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<LoginResponseModel> GetLoginModel(LoginKey key, CancellationToken cancellationToken)
        {
            return await this.blobService.DownloadBlobAsync<LoginResponseModel>(UserLoginPath, key.AccountId.ToString(), cancellationToken);
        }
    }
}
