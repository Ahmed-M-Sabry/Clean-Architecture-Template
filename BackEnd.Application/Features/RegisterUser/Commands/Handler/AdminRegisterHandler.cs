using AutoMapper;
using BackEnd.Application.Common;
using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Features.Login.Commands.Models;
using BackEnd.Application.Features.RegisterUser.Commands.Dtos;
using BackEnd.Application.Interfaces.Services;
using BackEnd.Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.Login.Commands.Handler
{
    public class EmployeeRegisterHandler : IRequestHandler<EmployeeRegisterCommand, Result<RegisterUserDto>>
    {
        private readonly IIdentityService _identityServies;
        private readonly IMapper _mapper;
        private readonly IAddSupUserService _SupUserService;

        public EmployeeRegisterHandler(
            IMapper mapper,
            IIdentityService identityServies
            , IAddSupUserService SupUserService
            )
        {
            _mapper = mapper;
            _identityServies = identityServies;
            _SupUserService = SupUserService;
        }

        public async Task<Result<RegisterUserDto>> Handle(EmployeeRegisterCommand request, CancellationToken cancellationToken)
        {

            //Email Exists
            var emailExists = await _identityServies.IsEmailExist(request.Email, cancellationToken);

            if (emailExists.IsSuccess && emailExists.Value)
            {
                return Result<RegisterUserDto>.Failure("Email already registered.", ErrorType.Conflict);
            }


            var newUser = _mapper.Map<ApplicationUser>(request);
            if (newUser == null)
            {
                return Result<RegisterUserDto>.Failure("User mapping failed.", ErrorType.InternalServerError);
            }

            var userResult = await _identityServies.CreateUserAsync(newUser, request.Password, ApplicationRoles.Employee, cancellationToken);
            if (!userResult.IsSuccess)
                return Result<RegisterUserDto>.Failure(userResult.Message, userResult.ErrorType);

            var Employee = new Employee(newUser.Id);
            var EmployeeResult = await _SupUserService.CreateEmployeeAsync(Employee, cancellationToken);
            if (!EmployeeResult.IsSuccess)
                return Result<RegisterUserDto>.Failure(EmployeeResult.Message, EmployeeResult.ErrorType);

            var token = await _identityServies.CreateJwtToken(newUser, cancellationToken);

           var response = new RegisterUserDto
           {
               UserId = newUser.Id,
               Email = newUser.Email,
               Token = token.Value,
               Role = ApplicationRoles.Employee
           };

            return Result<RegisterUserDto>.Success(response);
        }
    }

    public class DeafRegisterHandler : IRequestHandler<AdminRegisterCommand, Result<RegisterUserDto>>
    {
        private readonly IIdentityService _identityServies;
        private readonly IMapper _mapper;
        private readonly IAddSupUserService _SupUserService;

        public DeafRegisterHandler(
            IMapper mapper,
            IIdentityService identityServies,
            IAddSupUserService SupUserService)
        {
            _mapper = mapper;
            _identityServies = identityServies;
            _SupUserService = SupUserService;
        }

        public async Task<Result<RegisterUserDto>> Handle(AdminRegisterCommand request, CancellationToken cancellationToken)
        {

            var emailExists = await _identityServies.IsEmailExist(request.Email, cancellationToken);

            if (emailExists.IsSuccess && emailExists.Value)
            {
                return Result<RegisterUserDto>.Failure("Email already registered.", ErrorType.Conflict);
            }

            var newUser = _mapper.Map<ApplicationUser>(request);
            if (newUser == null)
            {
                return Result<RegisterUserDto>.Failure("User mapping failed.", ErrorType.InternalServerError);
            }

            var userResult = await _identityServies.CreateUserAsync(newUser, request.Password, ApplicationRoles.Admin, cancellationToken);
            if (!userResult.IsSuccess)
                return Result<RegisterUserDto>.Failure(userResult.Message, userResult.ErrorType);

            var token = await _identityServies.CreateJwtToken(newUser, cancellationToken);

            var response = new RegisterUserDto
            {
                UserId = newUser.Id,
                Email = newUser.Email,
                Token = token.Value,
                Role = ApplicationRoles.Admin
            };

            return Result<RegisterUserDto>.Success(response);
        }
    }
}
