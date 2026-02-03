using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Interfaces.Repositories.ProductRepo;
using BackEnd.Application.Interfaces.Services;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Infrastructure.Persistence.DbContext;
using BackEnd.Infrastructure.Persistence.Repositories;
using BackEnd.Infrastructure.Persistence.Repositories.ProductRepo;
using BackEnd.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Infrastructure.InfrastructureDependencies
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IIdentityService, IdentityServies>();
            services.AddScoped<IAddSupUserService, AddSupUserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped(
                    typeof(IGenericRepository<,>),
                    typeof(GenericRepository<,>)
                );
            return services;

        }
    }
}
