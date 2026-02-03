using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Domain.Entities.Identity.AuthenticationHepler;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.LoginUser.Command.Model
{
    public class UserLogInCommand : IRequest<Result<ResponseAuthModel>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
