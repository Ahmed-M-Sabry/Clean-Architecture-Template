using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Domain.Entities.Identity.AuthenticationHepler;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.RefreshToken.Model
{
    public class RefreshTokenCommand : IRequest<Result<ResponseAuthModel>>
    {

    }
}
