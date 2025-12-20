using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class KeInf
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<KEle> IdKEles { get; set; } = new List<KEle>();
}
