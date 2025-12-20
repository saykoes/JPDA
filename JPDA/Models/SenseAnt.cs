using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class SenseAnt
{
    public int Id { get; set; }

    public int? IdSense { get; set; }

    public string? Keb { get; set; }

    public string? Reb { get; set; }

    public int? SenseNumber { get; set; }

    public virtual Sense? IdSenseNavigation { get; set; }
}
