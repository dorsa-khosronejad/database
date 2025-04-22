using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebStore.Assignments;
using WebStore.Entities;

namespace WebStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Set up DI container
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDbContext<WebStoreContext>((sp, options) =>
                {
                    var connStr = configuration.GetConnectionString("WebStoreDb");
                    options.UseNpgsql(connStr);
                })
                .AddTransient<LinqQueriesAssignment>()
                .BuildServiceProvider();

            // 3. Resolve your assignment class (DbContext is injected automatically)
            var assignments = services.GetRequiredService<LinqQueriesAssignment>();

            // 4. Execute all LINQ query methods
            await assignments.Task01_ListAllCustomers();
            await assignments.Task02_ListOrdersWithItemCount();
            await assignments.Task03_ListProductsByDescendingPrice();
            await assignments.Task04_ListPendingOrdersWithTotalPrice();
            await assignments.Task05_OrderCountPerCustomer();
            await assignments.Task06_Top3CustomersByOrderValue();
            await assignments.Task07_RecentOrders();
            await assignments.Task08_TotalSoldPerProduct();
            await assignments.Task09_DiscountedOrders();
            await assignments.Task10_AdvancedQueryExample();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
