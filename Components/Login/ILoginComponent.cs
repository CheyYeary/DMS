using DMS.Models;

namespace DMS.Components.Login
{
    public interface ILoginComponent
    {
        Task<LoginResponseModel> Login(Guid accountId, CancellationToken cancellationToken);

        Task<LoginResponseModel> SignUp(Guid accountId, SignUpRequestModel signup, CancellationToken cancellationToken);

    }
}
