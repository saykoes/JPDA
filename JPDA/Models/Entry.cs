using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class Entry
{
    public int Id { get; set; }

    public virtual ICollection<KEle> KEles { get; set; } = new List<KEle>();

    public virtual ICollection<Pri> Pris { get; set; } = new List<Pri>();

    public virtual ICollection<REle> REles { get; set; } = new List<REle>();

    public virtual ICollection<Sense> Senses { get; set; } = new List<Sense>();
}
