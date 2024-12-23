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
    public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
    {
        public void Configure(EntityTypeBuilder<UserDetail> builder)
        {
            builder.ToTable("UserDetails");

            builder.HasKey(ud => ud.Id);
            builder.Property(ud => ud.Id).ValueGeneratedOnAdd();

            builder.Property(ud => ud.ProfileImageUrl)
                   .HasMaxLength(100);

            builder.HasOne(ud => ud.User)
                   .WithOne(u => u.UserDetail)
                   .HasForeignKey<UserDetail>(ud => ud.UserId);

        }
    }
}
