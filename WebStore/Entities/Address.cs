using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;  // ← for the attributes

namespace WebStore.Entities;

public partial class Address
{
    public Address()
    {
        OrderBillingAddresses  = new HashSet<Order>();
        OrderShippingAddresses = new HashSet<Order>();
    }

    public int AddressId { get; set; }
    // … other props …

    [InverseProperty(nameof(Order.BillingAddress))]
    public virtual ICollection<Order> OrderBillingAddresses { get; set; }

    [InverseProperty(nameof(Order.ShippingAddress))]
    public virtual ICollection<Order> OrderShippingAddresses { get; set; }

    // … customer nav etc …
}
