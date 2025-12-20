using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class ReInf
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<REle> IdREles { get; set; } = new List<REle>();
}
