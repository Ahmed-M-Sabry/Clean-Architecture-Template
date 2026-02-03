using BackEnd.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Domain.Entities.Identity
{
    public class Employee 
    {
        public string Id { get; private set; }
        public DateTime BirthDate { get; private set; }
        public int Age => DateTime.UtcNow.Year - BirthDate.Year;
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public Employee(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}
