using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class Field
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Sense> IdSenses { get; set; } = new List<Sense>();
}
