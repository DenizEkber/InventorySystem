using InventorySystem.CORE.Interfaces;
using InventorySystem.CORE.Services;
using InventorySystem.DATABASE.CodeFirst.Context;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using InventorySystem.Mappings;
using InventorySystem.UI;
using InventorySystem.UI.FormPanel;
using InventorySystem.UI.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InventorySystem
{
    internal static class Program
    {
        public static IHost Host { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // DbContext Registration
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer("Server=USER\\SQLEXPRESS;Database=InventorySystemDb;Trusted_Connection=True;TrustServerCertificate=True;"), ServiceLifetime.Scoped);


                    // Add AutoMapper
                    services.AddAutoMapper(typeof(MappingProfile));

                    //services.AddScoped<WarehouseDto>();

                    // Generic Repository and Service
                    services.AddScoped(typeof(BaseRepository<>));

                    // Service Registrations
                    services.AddScoped<IProductService, ProductService>();
                    services.AddScoped<ISupplierService, SupplierService>();
                    services.AddScoped<IStockTransactionService, StockTransactionService>();
                    services.AddScoped<ICategoryService, CategoryService>();
                    //services.AddScoped<ILocationService, LocationService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<IWarehouseService, WarehouseService>();
                    services.AddScoped<IStockService, StockService>();
                    services.AddScoped<ICurrentStockService, CurrentStockService>();
                    services.AddScoped<IDashboardService, DashboardService>();
                    services.AddScoped<CurrentStockSortSearchService>();

                    // Repository Registrations
                    services.AddScoped<ProductRepository>();
                    services.AddScoped< SupplierRepository>();
                    services.AddScoped<StockTransactionRepository>();
                    services.AddScoped<CategoryRepository>();
                    //services.AddScoped<LocationRepository>();
                    services.AddScoped<UserRepository>();
                    services.AddScoped<WarehouseRepository>();
                    services.AddScoped<StockRepository>();
                    services.AddScoped<CurrentStockRepository>();
                    services.AddScoped<UserDetailRepository>();

                    // Form Registration
                   services.AddTransient<MainForm>();
                    //services.AddTransient<ProductForm>();
                    services.AddTransient<WelcomeForm>();
                    //services.AddTransient<LoginForm>();
                    //services.AddTransient<RegisterForm>();
                    services.AddTransient<AddSupplierInfo>();
                    services.AddTransient<AddWareHouseInfo>();


                    services.AddScoped<BuyProductPanel>();
                    services.AddScoped<SalePanel>();
                    services.AddScoped<DashboardPanel>();

                })
                .Build();

            // Application Configuration
            ApplicationConfiguration.Initialize();

            using var scope = Host.Services.CreateScope();
            var welcomeForm = scope.ServiceProvider.GetRequiredService<WelcomeForm>();
            Application.Run(welcomeForm);
        }
    }
}
