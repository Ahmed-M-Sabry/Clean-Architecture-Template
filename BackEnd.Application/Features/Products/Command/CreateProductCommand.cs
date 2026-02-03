using AutoMapper;
using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Features.Products.Dto;
using BackEnd.Application.Interfaces.Services;
using BackEnd.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.Products.Command
{
    public record CreateProductCommand(
         string Name,
         string? Description,
         decimal Price
     ) : IRequest<Result<ReturnProductDto>>;

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ReturnProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public CreateProductHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<Result<ReturnProductDto>> Handle(CreateProductCommand request, CancellationToken ct)
        {
            var userId = _identityService.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Result<ReturnProductDto>.Failure("User not authenticated", ErrorType.Unauthorized);

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Price = request.Price,
                UserId = userId
            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(ct);

            var dto = _mapper.Map<ReturnProductDto>(product);

            return Result<ReturnProductDto>.Success(dto, "Product created successfully");
        }
    }
}
