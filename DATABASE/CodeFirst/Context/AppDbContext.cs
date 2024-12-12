using InventorySystem.DATABASE.CodeFirst.Configurations;
using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Context
{
    public class AppDbContext : DbContext
    {

        private readonly string connectionString = "Server=USER\\SQLEXPRESS;Database=InventorySystemDb;Trusted_Connection=True;TrustServerCertificate=True;";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new StockTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
