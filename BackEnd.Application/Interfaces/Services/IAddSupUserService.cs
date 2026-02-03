using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Interfaces.Services
{
    public interface IAddSupUserService
    {
        Task<Result<Employee>> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    }
}
