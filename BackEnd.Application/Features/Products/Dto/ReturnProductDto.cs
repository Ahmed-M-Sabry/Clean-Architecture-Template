using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Features.Products.Dto
{
    public class ReturnProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string UserId { get; internal set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
