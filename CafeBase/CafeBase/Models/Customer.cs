using System;
using System.Collections.Generic;

namespace CafeBase.Models;

public partial class Customer
{
    public int CustomerId { get; set; }
        
    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
