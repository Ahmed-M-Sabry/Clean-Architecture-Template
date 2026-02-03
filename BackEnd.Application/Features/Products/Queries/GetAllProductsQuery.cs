using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Abstractions.Queries;
using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Common.SearchCriteria;
using BackEnd.Application.Features.Products.Dto;
using BackEnd.Application.Interfaces.Repositories.ProductRepo;
using BackEnd.Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Application.Features.Products.Queries;

public class GetAllProductsQuery()
    : ProductSearchCriteria ,IRequest<Result<PagedResult<ReturnProductDto>>>;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, Result<PagedResult<ReturnProductDto>>>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsHandler(
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ReturnProductDto>>> Handle(
        GetAllProductsQuery request,
        CancellationToken ct)
    {

        var userId = _identityService.GetUserId();

        var pageIndex = request.PageIndex ?? 1;
        var pageSize = request.PageSize ?? 10;

        var query = _unitOfWork.ProductReadRepository.GetAllAsync();
        query = _unitOfWork.ProductReadRepository.GetAllBySearchCriteria(query, request);

        var total = await query.CountAsync(ct);
        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        var dtos = products.Select(p => new ReturnProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            UserId = p.UserId,
            CreatedOn = p.CreatedOn,
            IsActive = p.IsActive
        }).ToList();

        var pagedResult = new PagedResult<ReturnProductDto>(
            dtos.AsReadOnly(),
            total,
            pageIndex,
            pageSize
        );

        return Result<PagedResult<ReturnProductDto>>.Success(pagedResult);
    }
}