using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class KEle
{
    public int Id { get; set; }

    public int? IdEntry { get; set; }

    public int? IdPri { get; set; }

    public string? Keb { get; set; }

    public virtual Entry? IdEntryNavigation { get; set; }

    public virtual Pri? IdPriNavigation { get; set; }

    public virtual ICollection<KeInf> IdKeInfs { get; set; } = new List<KeInf>();

    public virtual ICollection<REle> IdREles { get; set; } = new List<REle>();
}
