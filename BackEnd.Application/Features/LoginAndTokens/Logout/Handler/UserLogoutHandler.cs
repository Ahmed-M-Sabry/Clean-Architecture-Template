using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.Logout.Command;
using BackEnd.Application.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.Logout.Handler
{
    public class UserLogoutHandler : IRequestHandler<UserLogoutCommand, Result<bool>>
    {
        private readonly IIdentityService _identityServies;
        public UserLogoutHandler(IIdentityService identityServies)
        {
            _identityServies = identityServies;
        }
        public async Task<Result<bool>> Handle(UserLogoutCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityServies.RevokeRefreshTokenFromCookiesAsync();
            if (!result.Value)
                return Result<bool>.Failure("Logout failed", ErrorType.InternalServerError);

            return Result<bool>.Success(true, "Logout Success");
        }
    }
}
