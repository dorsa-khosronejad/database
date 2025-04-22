using Microsoft.EntityFrameworkCore;
using WebStore.Entities;

namespace WebStore.Assignments
{
    public class LinqQueriesAssignment
    {
        private readonly WebStoreContext _dbContext;

        public LinqQueriesAssignment(WebStoreContext context)
        {
            _dbContext = context;
        }

        public async Task Task01_ListAllCustomers()
        {
            var customers = await _dbContext.Customers.ToListAsync();

            Console.WriteLine("=== TASK 01: List All Customers ===");
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}");
            }
        }

        public async Task Task02_ListOrdersWithItemCount()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer != null
                        ? o.Customer.FirstName + " " + o.Customer.LastName
                        : "Unknown",
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

        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            var pendingOrders = await _dbContext.Orders
                .Where(o => o.OrderStatus == "Pending")
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.OrderId,
                    FirstName = o.Customer != null ? o.Customer.FirstName : "Unknown",
                    LastName = o.Customer != null ? o.Customer.LastName : "Unknown",
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
                Console.WriteLine($"Customer Name: {(customerInfo != null ? customerInfo.FirstName + " " + customerInfo.LastName : "Unknown")}");
            }
        }

        public async Task Task07_RecentOrders()
        {
            var recentOrders = await _dbContext.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30))
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.Customer != null
                        ? o.Customer.FirstName + " " + o.Customer.LastName
                        : "Unknown"
                })
                .ToListAsync();

            Console.WriteLine("=== TASK 07: Recent Orders ===");
            foreach (var o in recentOrders)
            {
                Console.WriteLine($"Order ID: {o.OrderId}, Date: {o.OrderDate}, Customer: {o.CustomerName}");
            }
        }

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
                Console.WriteLine($"Product Name: {(productInfo != null ? productInfo.ProductName : "Unknown")}, Total Sold: {product.TotalSold}");
            }
        }

        public async Task Task09_DiscountedOrders()
        {
            var discountedOrders = await _dbContext.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Discount > 0))
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            Console.WriteLine("=== TASK 09: Discounted Orders ===");
            foreach (var order in discountedOrders)
            {
                var customerName = order.Customer != null
                    ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                    : "Unknown";
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {customerName}");
                foreach (var item in order.OrderItems.Where(oi => oi.Discount > 0))
                {
                    Console.WriteLine($"  Product: {(item.Product != null ? item.Product.ProductName : "Unknown")}, Discount: {item.Discount:C}");
                }
            }
        }

        public Task Task10_AdvancedQueryExample()
        {
            Console.WriteLine("=== TASK 10: Advanced Query Example ===");
            // Implement advanced query here if needed.
            return Task.CompletedTask;
        }
    }
}
