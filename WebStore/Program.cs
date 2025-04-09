using Microsoft.EntityFrameworkCore;
using WebStore.Assignments;
using WebStore.Entities;  // Uncomment this after generating your entities

namespace WebStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Ensure you have a valid DbContext connection
            var optionsBuilder = new DbContextOptionsBuilder<WebStoreContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=WebStore;Username=postgres;Password=Mandanaka88429323");

            // Initialize the DbContext
            using var context = new WebStoreContext(optionsBuilder.Options);

            // Initialize the LinqQueriesAssignment class
            var assignments = new LinqQueriesAssignment(context);

            // Execute all LINQ query methods
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
