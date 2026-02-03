using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Features.Products.Dto;
using BackEnd.Application.Interfaces.Repositories.ProductRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.Products.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<Result<ReturnProductDto>>;

    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ReturnProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ReturnProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
        {
            var product = await _unitOfWork.ProductReadRepository.GetByIdAsync(request.Id);

            if (product is null || product.IsDeleted)
                return Result<ReturnProductDto>.Failure("Product not found", ErrorType.NotFound);

            var dto = new ReturnProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                UserId = product.UserId,
                CreatedOn = product.CreatedOn
            };

            return Result<ReturnProductDto>.Success(dto);
        }
    }
}
