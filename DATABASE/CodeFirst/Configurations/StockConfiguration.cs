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
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.ToTable("Stocks");

            builder.HasKey(s => s.ID);

            builder.Property(s => s.SKU)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.Price)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.Product)
                   .WithMany()
                   .HasForeignKey(s => s.ProductID);
        }
    }
}
