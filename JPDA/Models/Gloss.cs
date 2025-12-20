using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class Gloss
{
    public int Id { get; set; }

    public int? IdSense { get; set; }

    public int IdLang { get; set; }

    public string? Content { get; set; }

    public virtual Lang? IdLangNavigation { get; set; }

    public virtual Sense? IdSenseNavigation { get; set; }
}
