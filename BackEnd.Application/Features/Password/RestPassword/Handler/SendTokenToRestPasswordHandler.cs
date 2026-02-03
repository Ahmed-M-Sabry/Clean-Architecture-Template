using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Interfaces.Services;
using ECommerce.Application.Features.AuthenticationFeatures.Password.RestPassword.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.AuthenticationFeatures.Password.RestPassword.Handler
{
    public class SendTokenToRestPasswordHandler : IRequestHandler<SendTokenToRestPasswordCommand, Result<ResetPasswordTokenResponse>>
    {
        private readonly IIdentityService _identityServies;

        public SendTokenToRestPasswordHandler(IIdentityService identityServies)
        {
            _identityServies = identityServies;
        }

        public async Task<Result<ResetPasswordTokenResponse>> Handle(SendTokenToRestPasswordCommand request, CancellationToken cancellationToken)
        {
            var userData = await _identityServies.GetUserByEmailAsync(request.Email);
            var user = userData.Value;
            if (user == null)
                return Result<ResetPasswordTokenResponse>.Failure("Can't Send Rest Password Token to this email", ErrorType.BadRequest);

            var restPasswordToken = await _identityServies.GetRestPasswordTokenAsync(user);

            var ResetPasswordResponse = new ResetPasswordTokenResponse
            {
                Token = restPasswordToken,
                UserId = user.Id.ToString()
            };
            return Result<ResetPasswordTokenResponse>.Success(ResetPasswordResponse,"Rest Password Token");
        }
    }
}
