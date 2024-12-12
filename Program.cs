using InventorySystem.CORE.Interfaces;
using InventorySystem.CORE.Services;
using InventorySystem.DATABASE.CodeFirst.Context;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.UI.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InventorySystem
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureServices((context, services) =>
            {
                // DbContext Registration
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer("Server=USER\\SQLEXPRESS;Database=InventorySystemDb;Trusted_Connection=True;TrustServerCertificate=True;"));

                // Service Registrations
                services.AddScoped<IProductService, ProductService>();
                services.AddScoped<ISupplierService, SupplierService>();
                services.AddScoped<IStockTransactionService, StockTransactionService>();

                // Repository Registrations
                services.AddScoped<ProductRepository>();
                services.AddScoped<SupplierRepository>();
                services.AddScoped<StockTransactionRepository>();

                // Form Registration
                services.AddTransient<MainForm>();
            });

            // Application Configuration
            ApplicationConfiguration.Initialize();
            var host = builder.Build();

            using var scope = host.Services.CreateScope();
            var mainForm = scope.ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
    }
}
