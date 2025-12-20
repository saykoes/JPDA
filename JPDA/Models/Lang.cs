using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class Lang
{
    public int Id { get; set; }

    public string? Iso3 { get; set; }

    public virtual ICollection<Gloss> Glosses { get; set; } = new List<Gloss>();
}
