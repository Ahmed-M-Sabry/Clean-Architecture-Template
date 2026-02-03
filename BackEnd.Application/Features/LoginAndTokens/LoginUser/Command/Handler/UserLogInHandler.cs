using AutoMapper;
using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.LoginUser.Command.Model;
using BackEnd.Application.Interfaces.Services;
using BackEnd.Domain.Entities.Identity.AuthenticationHepler;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.AuthenticationFeatures.LoginAndTokens.LoginUser.Command.Handler
{
    public class UserLogInHandler : IRequestHandler<UserLogInCommand, Result<ResponseAuthModel>>
    {
        private readonly IIdentityService _identityServies;
        private readonly IValidator<UserLogInCommand> _validator;
        private readonly IMapper _mapper;

        public UserLogInHandler(
            IValidator<UserLogInCommand> validator,
            IMapper mapper,
            IIdentityService identityServies)
        {
            _validator = validator;
            _mapper = mapper;
            _identityServies = identityServies;
        }
        public async Task<Result<ResponseAuthModel>> Handle(UserLogInCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if(!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<ResponseAuthModel>.Failure(string.Join(" | ", errors), ErrorType.BadRequest);
            }
            // if Email Not Exists
            var userData = await _identityServies.GetUserByEmailAsync(request.Email);
            var user = userData.Value; 
            if (user is null)
                return Result<ResponseAuthModel>.Failure("Invalid credentials", ErrorType.Unauthorized);

            //if (!user.EmailConfirmed)
            //{
            //    return Result<ResponseAuthModel>.Failure("Please confirm your email before logging in.", ErrorType.Unauthorized);
            //}

            // If Password incorrect
            var isPasswordValid = await _identityServies.IsPasswordExist(user, request.Password, cancellationToken);

            if (!isPasswordValid.Value)
                return Result<ResponseAuthModel>.Failure("Invalid credentials", ErrorType.Unauthorized);

            // Generate Refresh Token 
            var response = await _identityServies.GenerateRefreshTokenAsync(user, request.RememberMe, cancellationToken);

            return Result<ResponseAuthModel>.Success(response.Value);
        }
    }
}
