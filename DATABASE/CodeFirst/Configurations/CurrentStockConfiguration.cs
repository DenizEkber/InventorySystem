using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Configurations
{
    public class CurrentStockConfiguration : IEntityTypeConfiguration<CurrentStock>
    {
        public void Configure(EntityTypeBuilder<CurrentStock> builder)
        {
            builder.ToTable("CurrentStocks");

            builder.HasKey(cs => cs.ID);

            builder.Property(cs => cs.Price)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(cs => cs.Product)
                   .WithMany()
                   .HasForeignKey(cs => cs.ProductID);
        }
    }
}
