using BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.LoginUser.Command.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.LoginUser.Command.Validator
{
    public class UserLogInValidator : AbstractValidator<UserLogInCommand>
    {
        public UserLogInValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is Required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is Required.");

        }
    }
}
