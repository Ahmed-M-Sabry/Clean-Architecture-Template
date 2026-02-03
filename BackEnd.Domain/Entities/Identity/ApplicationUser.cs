using BackEnd.Domain.Common;
using BackEnd.Domain.Entities.Identity.AuthenticationHepler;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public bool? IsDelete { get; set; }
        public List<RefreshToken>? refreshTokens { get; set; }


        public Employee? Employee { get; set; }
    }
    
}
