using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.DATABASE.CodeFirst.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.WarehouseID);

            builder.Property(w => w.WarehouseName)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(w => w.ContactInfo)
                   .HasMaxLength(100);

            builder.HasOne(w => w.User)
                   .WithMany()
                   .HasForeignKey(w => w.UserID);
        }
    }
}
