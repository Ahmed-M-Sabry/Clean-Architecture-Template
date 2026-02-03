using AutoMapper;
using BackEnd.Application.Features.Products.Dto;
using BackEnd.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Mappings
{
    public class DomainMapping : Profile
    {
        public DomainMapping()
        {
            CreateMap<Product, ReturnProductDto>();
        }
    }
}
