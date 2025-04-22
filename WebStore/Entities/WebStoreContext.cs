using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebStore.Entities
{
    public partial class WebStoreContext : DbContext
    {
        private readonly IConfiguration? _configuration;

        public WebStoreContext(DbContextOptions<WebStoreContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public WebStoreContext(DbContextOptions<WebStoreContext> options)
            : base(options)
        {
        }

        public WebStoreContext()
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Staff> Staff { get; set; } = null!;
        public virtual DbSet<Stock> Stocks { get; set; } = null!;
        public virtual DbSet<Store> Stores { get; set; } = null!;
        public virtual DbSet<Carrier> Carriers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration != null)
            {
                var cs = _configuration.GetConnectionString("WebStoreConnection");
                optionsBuilder.UseNpgsql(cs!);
            }
            else
            {
                // Fallback for design-time or if IConfiguration not provided
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=WebStore;Username=postgres;Password=Mandanaka88429323");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Address ---
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.AddressId).HasName("addresses_pkey");
                entity.ToTable("addresses");
                // other Address props...
            });

            // --- OrderItem (composite key) ---
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderId)
                      .HasConstraintName("fk_orderitems_order");

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(e => e.ProductId)
                      .HasConstraintName("fk_orderitems_product");
            });

            // --- Order (billing/shipping addresses + tracking) ---
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("orders_pkey");
                entity.ToTable("orders");

                entity.HasOne(o => o.BillingAddress)
                      .WithMany(a => a.OrderBillingAddresses)
                      .HasForeignKey(o => o.BillingAddressId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("fk_orders_billing_address");

                entity.HasOne(o => o.ShippingAddress)
                      .WithMany(a => a.OrderShippingAddresses)
                      .HasForeignKey(o => o.ShippingAddressId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("fk_orders_shipping_address");

                entity.Property(o => o.TrackingNumber)
                      .HasColumnName("tracking_number")
                      .HasMaxLength(50);

                entity.Property(o => o.ShippedDate)
                      .HasColumnName("shipped_date");

                entity.Property(o => o.DeliveredDate)
                      .HasColumnName("delivered_date");
            });

            // --- Carrier ---
            modelBuilder.Entity<Carrier>(entity =>
            {
                entity.HasKey(e => e.CarrierId).HasName("carriers_pkey");
                entity.ToTable("carriers");

                entity.Property(e => e.CarrierName)
                      .HasColumnName("carrier_name")
                      .HasMaxLength(50);

                entity.Property(e => e.ContactUrl)
                      .HasColumnName("contact_url")
                      .HasMaxLength(50);

                entity.Property(e => e.ContactPhone)
                      .HasColumnName("contact_phone")
                      .HasMaxLength(50);

                entity.HasMany(c => c.Orders)
                      .WithOne(o => o.Carrier)
                      .HasForeignKey(o => o.CarrierId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
