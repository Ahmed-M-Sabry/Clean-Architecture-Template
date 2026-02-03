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
    public record UpdateProductCommand(
    int Id,
    string Name,
    string? Description,
    decimal Price
    ) : IRequest<Result<ReturnProductDto>>;
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ReturnProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public UpdateProductHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<Result<ReturnProductDto>> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var product = await _unitOfWork.ProductReadRepository.GetByIdAsync(request.Id);
            if (product is null || product.IsDeleted)
                return Result<ReturnProductDto>.Failure("Product not found or has been deleted", ErrorType.NotFound);

            var currentUserId = _identityService.GetUserId();
            if (product.UserId != currentUserId)
                return Result<ReturnProductDto>.Failure("You do not have permission to update this product", ErrorType.Forbidden);

            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim();
            product.Price = request.Price;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(ct);

            var dto = _mapper.Map<ReturnProductDto>(product);

            return Result<ReturnProductDto>.Success(dto, "Product updated successfully");
        }
    }

}
