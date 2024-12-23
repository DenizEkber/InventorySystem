/*using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Configurations
{
    public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
    {
        public void Configure(EntityTypeBuilder<PriceHistory> builder)
        {
            builder.HasKey(ph => ph.PriceID);

            builder.HasOne(ph => ph.Stock)
                .WithMany()
                .HasForeignKey(ph => ph.StockID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ph => ph.Price)
                .IsRequired();

            builder.Property(ph => ph.StartDate)
                .IsRequired();

            builder.Property(ph => ph.EndDate)
                .IsRequired(false); // EndDate optional
        }
    }
}
*/