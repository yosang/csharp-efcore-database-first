using System;
using System.Collections.Generic;

namespace test.Models;

public partial class Tool
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}
