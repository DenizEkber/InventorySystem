using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.DATABASE.CodeFirst.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(s => s.ContactInfo)
                   .HasMaxLength(100);

            builder.HasOne(s => s.User)
                   .WithMany()
                   .HasForeignKey(s => s.UserID);
        }
    }
}
