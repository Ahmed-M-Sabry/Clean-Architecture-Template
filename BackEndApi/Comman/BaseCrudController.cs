using BackEnd.Application.Common;
using BackEnd.Application.Common.ResponseFormat;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Api.Common
{
    public abstract class BaseCrudController<
    TCreateCommand, TCreateResponse,
    TUpdateCommand, TUpdateResponse,
    TDeleteCommand, TDeleteResponse,
    TGetByIdQuery, TGetByIdResponse,
    TGetAllQuery, TGetAllResponse> : ControllerBase
    where TCreateCommand : IRequest<Result<TCreateResponse>>
    where TUpdateCommand : IRequest<Result<TUpdateResponse>>
    where TDeleteCommand : IRequest<Result<TDeleteResponse>>
    where TGetByIdQuery : IRequest<Result<TGetByIdResponse>>
    where TGetAllQuery : IRequest<Result<TGetAllResponse>>
    {
        protected readonly IMediator _mediator;

        protected BaseCrudController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TCreateCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPut]
        public virtual async Task<IActionResult> Update([FromBody] TUpdateCommand command)
            => Ok(await _mediator.Send(command));

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] TDeleteCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute] TGetByIdQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet]
        public virtual async Task<IActionResult> GetAll([FromQuery] TGetAllQuery query)
        {
            query ??= Activator.CreateInstance<TGetAllQuery>();
            return Ok(await _mediator.Send(query));
        }
    }

}
