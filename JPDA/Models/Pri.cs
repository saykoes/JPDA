using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class Pri
{
    public int Id { get; set; }

    public int? IdEntry { get; set; }

    public int? News { get; set; }

    public int? Ichi { get; set; }

    public int? Spec { get; set; }

    public int? Gai { get; set; }

    public int? Nf { get; set; }

    public virtual Entry? IdEntryNavigation { get; set; }

    public virtual ICollection<KEle> KEles { get; set; } = new List<KEle>();

    public virtual ICollection<REle> REles { get; set; } = new List<REle>();
}
