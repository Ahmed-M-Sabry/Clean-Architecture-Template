using BackEnd.Domain.Entities;
using BackEnd.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Infrastructure.Persistence.Configurations
{
    public class EmployeeConfigurations :  IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                  .ValueGeneratedNever();

            builder.HasOne(e => e.ApplicationUser)
                  .WithOne(u => u.Employee)
                  .HasForeignKey<Employee>(e => e.Id);
        }
    }
}
