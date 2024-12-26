using System;
using System.Collections.Generic;

namespace CafeBase.Models;

public partial class MenuItem
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal Price { get; set; }

    public bool? IsAvailable { get; set; }

    public string? Img { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
