using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Features.AuthenticationFeatures.Password.RestPassword.Command;
using BackEnd.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.Password.RestPassword.Handler
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IIdentityService _identityServies;

        public ResetPasswordHandler(IIdentityService identityServies)
        {
            _identityServies = identityServies;
        }


        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var userData = await _identityServies.IsUserExist(request.UserId);
            var user = userData.Value;
            if (user == null)
                return Result<string>.Failure("Invalid user ID.", ErrorType.NotFound);

            var tokenBytes = WebEncoders.Base64UrlDecode(request.Token);
            var decodedToken = Encoding.UTF8.GetString(tokenBytes);

            var result = await _identityServies.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return Result<string>.Failure(errors, ErrorType.BadRequest);
            }

            return Result<string>.Success("Password reset successfully!");
        }
    }

}
