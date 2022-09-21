using DMS.Components.Login;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers;

[ApiController]
[Route("api/")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> logger;
    private readonly ILoginComponent loginComponent;

    public LoginController(ILogger<LoginController> logger, ILoginComponent loginComponent)
    {
        this.logger = logger;
        this.loginComponent = loginComponent;
    }

    [HttpPut]
    [Route("Login")]
    public async Task<IActionResult> Login(
        [FromHeader] Guid accountId,
        CancellationToken cancellationToken)
    {
        LoginResponseModel result = await this.loginComponent.Login(accountId, cancellationToken);

        return this.Ok(result);
    }

    [HttpPut]
    [Route("SignUp")]
    public async Task<IActionResult> SignUp(
        [FromHeader] Guid accountId,
        [FromBody] SignUpRequestModel body,
        CancellationToken cancellationToken)
    {
        // TODO: format a resource URI used to get the login
        string resourceUri = "";
        // TODO: read the accountId from the request
        LoginResponseModel result = await this.loginComponent.SignUp(accountId, body, cancellationToken);

        return this.Created(resourceUri, result);
    }
}
