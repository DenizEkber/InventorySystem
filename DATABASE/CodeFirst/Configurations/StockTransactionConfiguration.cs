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
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.ToTable("StockTransactions");

            builder.HasKey(st => st.ID);

            builder.Property(st => st.Quantity)
                   .IsRequired();

            builder.Property(st => st.TransactionDate)
                   .IsRequired();

            builder.HasOne(st => st.Stock)
                   .WithMany()
                   .HasForeignKey(st => st.StockID);
        }
    }
}
