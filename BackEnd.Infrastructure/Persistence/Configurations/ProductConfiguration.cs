using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackEnd.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

            builder.Property(p => p.Description)
                    .HasMaxLength(1000);

            builder.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

            builder.HasIndex(p => p.Name);

            builder.Property(p => p.CreatedOn)
                    .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedOn)
                    .IsRequired(false);

            builder.HasOne(p => p.User)
               .WithMany() 
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

