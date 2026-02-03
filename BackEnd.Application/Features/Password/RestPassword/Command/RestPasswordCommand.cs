using BackEnd.Application.Common.ResponseFormat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.Password.RestPassword.Command
{
    public class ResetPasswordCommand : IRequest<Result<string>>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }

    }

}
