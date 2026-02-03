using BackEnd.Application.Interfaces.Repositories.ProductRepo;
using BackEnd.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        public IProductReadRepository ProductReadRepository { get; }
        public IProductRepository ProductRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
