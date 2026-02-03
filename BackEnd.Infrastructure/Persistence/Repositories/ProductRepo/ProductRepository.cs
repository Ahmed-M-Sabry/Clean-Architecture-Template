using BackEnd.Application.Interfaces.Repositories.ProductRepo;
using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Infrastructure.Persistence.Repositories.ProductRepo
{
    public class ProductRepository: GenericRepository<Product, int>,IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
