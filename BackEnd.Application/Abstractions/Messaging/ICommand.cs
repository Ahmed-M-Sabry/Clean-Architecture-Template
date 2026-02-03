using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Abstractions.Messaging
{
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
