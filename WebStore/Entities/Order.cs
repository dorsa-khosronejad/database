using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Entities;

public partial class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }

    public int ShippingAddressId { get; set; }
    public int BillingAddressId  { get; set; }

    // Now nullable
    public string?   OrderStatus { get; set; }
    public DateTime? OrderDate   { get; set; }

    [ForeignKey(nameof(BillingAddressId))]
    [InverseProperty(nameof(Address.OrderBillingAddresses))]
    public virtual Address BillingAddress { get; set; } = null!;

    [ForeignKey(nameof(ShippingAddressId))]
    [InverseProperty(nameof(Address.OrderShippingAddresses))]
    public virtual Address ShippingAddress { get; set; } = null!;

    public virtual Customer? Customer    { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public int?        CarrierId      { get; set; }
    public string?     TrackingNumber { get; set; }
    public DateTime?   ShippedDate    { get; set; }
    public DateTime?   DeliveredDate  { get; set; }
    public virtual    Carrier? Carrier { get; set; }
    
}
