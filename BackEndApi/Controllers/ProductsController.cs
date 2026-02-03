using BackEnd.Api.Comman;
using BackEnd.Api.Common;
using BackEnd.Application.Abstractions.Queries;
using BackEnd.Application.Features.Products.Command;
using BackEnd.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApplicationControllerBase
    {
        public ProductsController(IMediator mediator) : base(mediator) { }

        [HttpPost("Create-Product")]
        public virtual async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("Update-Product")]
        public virtual async Task<IActionResult> Update([FromBody] UpdateProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("Delete-Product")]
        public virtual async Task<IActionResult> Delete([FromBody] DeleteProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("Get-Product-ById")]
        public virtual async Task<IActionResult> GetById([FromQuery] GetProductByIdQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost("Product-Search")]
        public async Task<IActionResult> Search([FromBody] GetAllProductsQuery query)
        {
            query ??= new GetAllProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

}
