using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Api.Comman
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApplicationControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected ApplicationControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
