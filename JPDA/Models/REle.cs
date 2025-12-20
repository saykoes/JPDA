using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class REle
{
    public int Id { get; set; }

    public int? IdEntry { get; set; }

    public int? IdPri { get; set; }

    public string? Reb { get; set; }

    public virtual Entry? IdEntryNavigation { get; set; }

    public virtual Pri? IdPriNavigation { get; set; }

    public virtual ICollection<KEle> IdKEles { get; set; } = new List<KEle>();

    public virtual ICollection<ReInf> IdReInfs { get; set; } = new List<ReInf>();
}
