using System;
using System.Collections.Generic;

namespace JPDA.Models;

public partial class Sense
{
    public int Id { get; set; }

    public int? IdEntry { get; set; }

    public virtual ICollection<Gloss> Glosses { get; set; } = new List<Gloss>();

    public virtual Entry? IdEntryNavigation { get; set; }

    public virtual ICollection<SenseAnt> SenseAnts { get; set; } = new List<SenseAnt>();

    public virtual ICollection<SenseXref> SenseXrefs { get; set; } = new List<SenseXref>();

    public virtual ICollection<Dial> IdDials { get; set; } = new List<Dial>();

    public virtual ICollection<Field> IdFields { get; set; } = new List<Field>();

    public virtual ICollection<Misc> IdMiscs { get; set; } = new List<Misc>();

    public virtual ICollection<Po> IdPos { get; set; } = new List<Po>();
}
