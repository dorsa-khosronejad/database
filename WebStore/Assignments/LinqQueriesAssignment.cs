using Microsoft.EntityFrameworkCore;
using WebStore.Entities;

namespace WebStore.Assignments
{
    /// <summary>
    /// This class demonstrates various LINQ query tasks 
    /// to practice querying an EF Core database.
    /// 
    /// ASSIGNMENT INSTRUCTIONS:
    ///   1. For each method labeled "TODO", write the necessary
    ///      LINQ query to return or display the required data.
    ///      
    ///   2. Print meaningful output to the console (or return
    ///      collections, as needed).
    ///      
    ///   3. Test each method by calling it from your Program.cs
    ///      or test harness.
    /// </summary>
    public class LinqQueriesAssignment
    {
        private readonly WebStoreContext _dbContext;

        public LinqQueriesAssignment(WebStoreContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// 1. List all customers in the database:
        ///    - Print each customer's full name (First + Last) and Email.
        /// </summary>
        public async Task Task01_ListAllCustomers()
        {
            var customers = await _dbContext.Customers
               .ToListAsync();

            Console.WriteLine("=== TASK 01: List All Customers ===");
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}");
            }
        }

        /// <summary>
        /// 2. Fetch all orders along with:
        ///    - Customer Name
        ///    - Order ID
        ///    - Order Status
        ///    - Number of items in each order (the sum of OrderItems.Quantity)
        /// </summary>
        public async Task Task02_ListOrdersWithItemCount()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    o.OrderStatus,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            Console.WriteLine("=== TASK 02: List Orders With Item Count ===");
            foreach (var o in orders)
            {
                Console.WriteLine($"Order ID: {o.OrderId}, Customer: {o.CustomerName}, Status: {o.OrderStatus}, Items: {o.ItemCount}");
            }
        }

        /// <summary>
        /// 3. List all products (ProductName, Price),
        ///    sorted by price descending (highest first).
        /// </summary>
        public async Task Task03_ListProductsByDescendingPrice()
        {
            var products = await _dbContext.Products
                .OrderByDescending(p => p.Price)
                .ToListAsync();

            Console.WriteLine("=== TASK 03: List Products By Descending Price ===");
            foreach (var p in products)
            {
                Console.WriteLine($"Product: {p.ProductName}, Price: {p.Price:C}");
            }
        }

        /// <summary>
        /// 4. Find all "Pending" orders (order status = "Pending")
        ///    and display:
        ///      - Customer Name
        ///      - Order ID
        ///      - Order Date
        ///      - Total price (sum of unit_price * quantity - discount) for each order
        /// </summary>
        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            var pendingOrders = await _dbContext.Orders
                .Where(o => o.OrderStatus == "Pending")
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.OrderId,
                    o.Customer.FirstName,
                    o.Customer.LastName,
                    o.OrderDate,
                    TotalPrice = o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount)
                })
                .ToListAsync();

            Console.WriteLine("=== TASK 04: List Pending Orders With Total Price ===");
            foreach (var o in pendingOrders)
            {
                Console.WriteLine($"Order ID: {o.OrderId}, Customer: {o.FirstName} {o.LastName}, Date: {o.OrderDate}, Total Price: {o.TotalPrice:C}");
            }
        }

        /// <summary>
        /// 5. List the total number of orders each customer has placed.
        ///    Output should show:
        ///      - Customer Full Name
        ///      - Number of Orders
        /// </summary>
        public async Task Task05_OrderCountPerCustomer()
        {
            var orderCount = await _dbContext.Customers
                .Select(c => new
                {
                    c.FirstName,
                    c.LastName,
                    OrderCount = c.Orders.Count
                })
                .ToListAsync();

            Console.WriteLine("=== TASK 05: Order Count Per Customer ===");
            foreach (var c in orderCount)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName}: {c.OrderCount} Orders");
            }
        }

        /// <summary>
        /// 6. Show the top 3 customers who have placed the highest total order value overall.
        ///    - For each customer, calculate SUM of (OrderItems * Price).
        ///      Then pick the top 3.
        /// </summary>
        public async Task Task06_Top3CustomersByOrderValue()
        {
            var topCustomers = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalOrderValue = g.Sum(o => o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount))
                })
                .OrderByDescending(x => x.TotalOrderValue)
                .Take(3)
                .ToListAsync();

            Console.WriteLine("=== TASK 06: Top 3 Customers By Order Value ===");
            foreach (var customer in topCustomers)
            {
                var customerInfo = await _dbContext.Customers.FindAsync(customer.CustomerId);
              Console.WriteLine($"Customer Name: {customerInfo?.FirstName} {customerInfo?.LastName ?? "Unknown"}");
            }
        }

        /// <summary>
        /// 7. Show all orders placed in the last 30 days (relative to now).
        ///    - Display order ID, date, and customer name.
        /// </summary>
        public async Task Task07_RecentOrders()
        {
            var recentOrders = await _dbContext.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30))
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName
                })
                .ToListAsync();

            Console.WriteLine("=== TASK 07: Recent Orders ===");
            foreach (var o in recentOrders)
            {
                Console.WriteLine($"Order ID: {o.OrderId}, Date: {o.OrderDate}, Customer: {o.CustomerName}");
            }
        }

        /// <summary>
        /// 8. For each product, display how many total items have been sold
        ///    across all orders.
        ///    - Product name, total sold quantity.
        ///    - Sort by total sold descending.
        /// </summary>
        public async Task Task08_TotalSoldPerProduct()
        {
            var totalSoldPerProduct = await _dbContext.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToListAsync();

            Console.WriteLine("=== TASK 08: Total Sold Per Product ===");
            foreach (var product in totalSoldPerProduct)
            {
                var productInfo = await _dbContext.Products.FindAsync(product.ProductId);
                Console.WriteLine($"Product Name: {productInfo?.ProductName ?? "Unknown"}");
            }
        }

        /// <summary>
        /// 9. List any orders that have at least one OrderItem with a Discount > 0.
        ///    - Show Order ID, Customer name, and which products were discounted.
        /// </summary>
        public async Task Task09_DiscountedOrders()
        {
            var discountedOrders = await _dbContext.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Discount > 0))
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToListAsync();

            Console.WriteLine("=== TASK 09: Discounted Orders ===");
            foreach (var order in discountedOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.Customer.FirstName} {order.Customer.LastName}");
                foreach (var item in order.OrderItems.Where(oi => oi.Discount > 0))
                {
                    Console.WriteLine($"  Product: {item.Product.ProductName}, Discount: {item.Discount:C}");
                }
            }
        }

        /// <summary>
/// 10. (Open-ended) Combine multiple joins or navigation properties
///     to retrieve a more complex set of data.
/// </summary>
public Task Task10_AdvancedQueryExample()
{
    Console.WriteLine("=== TASK 10: Advanced Query Example ===");
    // You can implement the logic for the complex query here.
    return Task.CompletedTask;
}
    }
}
