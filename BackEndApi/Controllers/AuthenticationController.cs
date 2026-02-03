using Azure;
using BackEnd.Api.Comman;
using BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.LoginUser.Command.Model;
using BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.Logout.Command;
using BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.RefreshToken.Model;
using BackEnd.Application.Features.AuthenticationFeatures.Password.RestPassword.Command;
using BackEnd.Application.Features.Login.Commands.Models;
using ECommerce.Application.Features.AuthenticationFeatures.Password.RestPassword.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApplicationControllerBase
    {
        public AuthenticationController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("Register-Admin")]
        public async Task<IActionResult> RegisterVolunteer([FromForm] AdminRegisterCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Register-Employee")]
        public async Task<IActionResult> RegisterDeafUser([FromForm] EmployeeRegisterCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromForm] UserLogInCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess &&
                result.Value?.RefreshToken != null &&
                result.Value.CookieOptions != null)
            {
                Response.Cookies.Append(
                    "RefreshToken",
                    result.Value.RefreshToken,
                    result.Value.CookieOptions
                );
            }

            return Ok(result);
        }

        [HttpPost("Generate-New-token-From-RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _mediator.Send(new RefreshTokenCommand());

            if (result.IsSuccess &&
                result.Value?.RefreshToken != null &&
                result.Value.CookieOptions != null)
            {
                Response.Cookies.Delete("RefreshToken");
                Response.Cookies.Append(
                    "RefreshToken",
                    result.Value.RefreshToken,
                    result.Value.CookieOptions
                );
            }

            return Ok(result);
        }

        [HttpPost("Rest-Password")]
        public async Task<IActionResult> RestPassword([FromForm] ResetPasswordCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword([FromForm] SendTokenToRestPasswordCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(await _mediator.Send(new UserLogoutCommand()));
        }
    }
}
