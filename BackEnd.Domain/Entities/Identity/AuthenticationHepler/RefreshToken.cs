using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Domain.Entities.Identity.AuthenticationHepler
{
    [Owned] // Add Automatic id the table owned to Application user table
    public class RefreshToken
    {
        public string token { get; set; }
        public DateTime ExpireOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpireOn;
        public bool IsActive => RevokedOn == null && !IsExpired;
    }
}
