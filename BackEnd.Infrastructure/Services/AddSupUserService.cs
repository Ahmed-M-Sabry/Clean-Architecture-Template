using BackEnd.Application.Common;
using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Interfaces.Services;
using BackEnd.Domain.Entities.Identity;
using BackEnd.Infrastructure.Persistence.DbContext;


namespace BackEnd.Infrastructure.Services
{
    public class AddSupUserService : IAddSupUserService
    {
        private readonly ApplicationDbContext _context;

        public AddSupUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Employee>> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            await _context.Employees.AddAsync(employee, cancellationToken);
            var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!saved)
                return Result<Employee>.Failure("Failed to create Employee", ErrorType.InternalServerError);

            return Result<Employee>.Success(employee);
        }
    }
}
