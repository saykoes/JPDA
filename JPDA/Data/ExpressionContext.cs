using System;
using System.Collections.Generic;
using JPDA.Models;
using JPDA.Views;
using Microsoft.EntityFrameworkCore;

namespace JPDA.Data;

public partial class ExpressionContext : DbContext
{
    public ExpressionContext()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public ExpressionContext(DbContextOptions<ExpressionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dial> Dials { get; set; }

    public virtual DbSet<Entry> Entries { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<Gloss> Glosses { get; set; }

    public virtual DbSet<KEle> KEles { get; set; }

    public virtual DbSet<KeInf> KeInfs { get; set; }

    public virtual DbSet<Lang> Langs { get; set; }

    public virtual DbSet<Misc> Miscs { get; set; }

    public virtual DbSet<Po> Pos { get; set; }

    public virtual DbSet<Pri> Pris { get; set; }

    public virtual DbSet<REle> REles { get; set; }

    public virtual DbSet<ReInf> ReInfs { get; set; }

    public virtual DbSet<Sense> Senses { get; set; }

    public virtual DbSet<SenseAnt> SenseAnts { get; set; }

    public virtual DbSet<SenseXref> SenseXrefs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"DataSource={MainView.DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dial>(entity =>
        {
            entity.ToTable("dial");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("STRING")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("STRING")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Entry>(entity =>
        {
            entity.ToTable("entry");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.ToTable("field");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("STRING")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("STRING")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Gloss>(entity =>
        {
            entity.ToTable("gloss");

            entity.HasIndex(e => new { e.Content, e.IdSense }, "idx_gloss_content_sense");

            entity.HasIndex(e => e.IdSense, "idx_gloss_id_sense");

            entity.HasIndex(e => new { e.IdSense, e.Content, e.IdLang }, "idx_gloss_sense_content_lang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("STRING")
                .HasColumnName("content");
            entity.Property(e => e.IdLang).HasColumnName("id_lang");
            entity.Property(e => e.IdSense).HasColumnName("id_sense");

            entity.HasOne(d => d.IdLangNavigation).WithMany(p => p.Glosses).HasForeignKey(d => d.IdLang);

            entity.HasOne(d => d.IdSenseNavigation).WithMany(p => p.Glosses).HasForeignKey(d => d.IdSense);
        });

        modelBuilder.Entity<KEle>(entity =>
        {
            entity.ToTable("k_ele");

            entity.HasIndex(e => new { e.IdEntry, e.Keb }, "idx_k_ele_entry_keb");

            entity.HasIndex(e => e.IdEntry, "idx_k_ele_id_entry");

            entity.HasIndex(e => new { e.Keb, e.IdEntry }, "idx_k_ele_keb_entry");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdEntry).HasColumnName("id_entry");
            entity.Property(e => e.IdPri).HasColumnName("id_pri");
            entity.Property(e => e.Keb)
                .HasColumnType("STRING")
                .HasColumnName("keb");

            entity.HasOne(d => d.IdEntryNavigation).WithMany(p => p.KEles).HasForeignKey(d => d.IdEntry);

            entity.HasOne(d => d.IdPriNavigation).WithMany(p => p.KEles).HasForeignKey(d => d.IdPri);

            entity.HasMany(d => d.IdKeInfs).WithMany(p => p.IdKEles)
                .UsingEntity<Dictionary<string, object>>(
                    "KEleKeInf",
                    r => r.HasOne<KeInf>().WithMany()
                        .HasForeignKey("IdKeInf")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<KEle>().WithMany()
                        .HasForeignKey("IdKEle")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdKEle", "IdKeInf");
                        j.ToTable("k_ele_ke_inf");
                        j.IndexerProperty<int>("IdKEle").HasColumnName("id_k_ele");
                        j.IndexerProperty<int>("IdKeInf").HasColumnName("id_ke_inf");
                    });
        });

        modelBuilder.Entity<KeInf>(entity =>
        {
            entity.ToTable("ke_inf");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("STRING")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("STRING")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Lang>(entity =>
        {
            entity.ToTable("lang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Iso3)
                .HasColumnType("STRING")
                .HasColumnName("iso3");
        });

        modelBuilder.Entity<Misc>(entity =>
        {
            entity.ToTable("misc");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("STRING")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("STRING")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Po>(entity =>
        {
            entity.ToTable("pos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("STRING")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("STRING")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Pri>(entity =>
        {
            entity.ToTable("pri");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Gai).HasColumnName("gai");
            entity.Property(e => e.Ichi).HasColumnName("ichi");
            entity.Property(e => e.IdEntry).HasColumnName("id_entry");
            entity.Property(e => e.News).HasColumnName("news");
            entity.Property(e => e.Nf).HasColumnName("nf");
            entity.Property(e => e.Spec).HasColumnName("spec");

            entity.HasOne(d => d.IdEntryNavigation).WithMany(p => p.Pris).HasForeignKey(d => d.IdEntry);
        });

        modelBuilder.Entity<REle>(entity =>
        {
            entity.ToTable("r_ele");

            entity.HasIndex(e => new { e.IdEntry, e.Reb }, "idx_r_ele_entry_reb");

            entity.HasIndex(e => e.IdEntry, "idx_r_ele_id_entry");

            entity.HasIndex(e => new { e.Reb, e.IdEntry }, "idx_r_ele_reb_entry");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdEntry).HasColumnName("id_entry");
            entity.Property(e => e.IdPri).HasColumnName("id_pri");
            entity.Property(e => e.Reb)
                .HasColumnType("STRING")
                .HasColumnName("reb");

            entity.HasOne(d => d.IdEntryNavigation).WithMany(p => p.REles).HasForeignKey(d => d.IdEntry);

            entity.HasOne(d => d.IdPriNavigation).WithMany(p => p.REles).HasForeignKey(d => d.IdPri);

            entity.HasMany(d => d.IdKEles).WithMany(p => p.IdREles)
                .UsingEntity<Dictionary<string, object>>(
                    "REleKEle",
                    r => r.HasOne<KEle>().WithMany()
                        .HasForeignKey("IdKEle")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<REle>().WithMany()
                        .HasForeignKey("IdREle")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdREle", "IdKEle");
                        j.ToTable("r_ele_k_ele");
                        j.IndexerProperty<int>("IdREle").HasColumnName("id_r_ele");
                        j.IndexerProperty<int>("IdKEle").HasColumnName("id_k_ele");
                    });

            entity.HasMany(d => d.IdReInfs).WithMany(p => p.IdREles)
                .UsingEntity<Dictionary<string, object>>(
                    "REleReInf",
                    r => r.HasOne<ReInf>().WithMany()
                        .HasForeignKey("IdReInf")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<REle>().WithMany()
                        .HasForeignKey("IdREle")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdREle", "IdReInf");
                        j.ToTable("r_ele_re_inf");
                        j.IndexerProperty<int>("IdREle").HasColumnName("id_r_ele");
                        j.IndexerProperty<int>("IdReInf").HasColumnName("id_re_inf");
                    });
        });

        modelBuilder.Entity<ReInf>(entity =>
        {
            entity.ToTable("re_inf");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("STRING")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("STRING")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Sense>(entity =>
        {
            entity.ToTable("sense");

            entity.HasIndex(e => e.IdEntry, "idx_sense_id_entry");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdEntry).HasColumnName("id_entry");

            entity.HasOne(d => d.IdEntryNavigation).WithMany(p => p.Senses).HasForeignKey(d => d.IdEntry);

            entity.HasMany(d => d.IdDials).WithMany(p => p.IdSenses)
                .UsingEntity<Dictionary<string, object>>(
                    "SenseDial",
                    r => r.HasOne<Dial>().WithMany()
                        .HasForeignKey("IdDial")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Sense>().WithMany()
                        .HasForeignKey("IdSense")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdSense", "IdDial");
                        j.ToTable("sense_dial");
                        j.HasIndex(new[] { "IdSense" }, "idx_sense_dial_sense");
                        j.IndexerProperty<int>("IdSense").HasColumnName("id_sense");
                        j.IndexerProperty<int>("IdDial").HasColumnName("id_dial");
                    });

            entity.HasMany(d => d.IdFields).WithMany(p => p.IdSenses)
                .UsingEntity<Dictionary<string, object>>(
                    "SenseField",
                    r => r.HasOne<Field>().WithMany()
                        .HasForeignKey("IdField")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Sense>().WithMany()
                        .HasForeignKey("IdSense")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdSense", "IdField");
                        j.ToTable("sense_field");
                        j.HasIndex(new[] { "IdSense" }, "idx_sense_field_sense");
                        j.IndexerProperty<int>("IdSense").HasColumnName("id_sense");
                        j.IndexerProperty<int>("IdField").HasColumnName("id_field");
                    });

            entity.HasMany(d => d.IdMiscs).WithMany(p => p.IdSenses)
                .UsingEntity<Dictionary<string, object>>(
                    "SenseMisc",
                    r => r.HasOne<Misc>().WithMany()
                        .HasForeignKey("IdMisc")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Sense>().WithMany()
                        .HasForeignKey("IdSense")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdSense", "IdMisc");
                        j.ToTable("sense_misc");
                        j.HasIndex(new[] { "IdSense" }, "idx_sense_misc_sense");
                        j.IndexerProperty<int>("IdSense").HasColumnName("id_sense");
                        j.IndexerProperty<int>("IdMisc").HasColumnName("id_misc");
                    });

            entity.HasMany(d => d.IdPos).WithMany(p => p.IdSenses)
                .UsingEntity<Dictionary<string, object>>(
                    "SensePo",
                    r => r.HasOne<Po>().WithMany()
                        .HasForeignKey("IdPos")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Sense>().WithMany()
                        .HasForeignKey("IdSense")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdSense", "IdPos");
                        j.ToTable("sense_pos");
                        j.HasIndex(new[] { "IdSense" }, "idx_sense_pos_sense");
                        j.IndexerProperty<int>("IdSense").HasColumnName("id_sense");
                        j.IndexerProperty<int>("IdPos").HasColumnName("id_pos");
                    });
        });

        modelBuilder.Entity<SenseAnt>(entity =>
        {
            entity.ToTable("sense_ant");

            entity.HasIndex(e => e.IdSense, "idx_sense_ant_sense");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdSense).HasColumnName("id_sense");
            entity.Property(e => e.Keb)
                .HasColumnType("STRING")
                .HasColumnName("keb");
            entity.Property(e => e.Reb)
                .HasColumnType("STRING")
                .HasColumnName("reb");
            entity.Property(e => e.SenseNumber).HasColumnName("sense_number");

            entity.HasOne(d => d.IdSenseNavigation).WithMany(p => p.SenseAnts).HasForeignKey(d => d.IdSense);
        });

        modelBuilder.Entity<SenseXref>(entity =>
        {
            entity.ToTable("sense_xref");

            entity.HasIndex(e => e.IdSense, "idx_sense_xref_sense");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdSense).HasColumnName("id_sense");
            entity.Property(e => e.Keb)
                .HasColumnType("STRING")
                .HasColumnName("keb");
            entity.Property(e => e.Reb)
                .HasColumnType("STRING")
                .HasColumnName("reb");
            entity.Property(e => e.SenseNumber).HasColumnName("sense_number");

            entity.HasOne(d => d.IdSenseNavigation).WithMany(p => p.SenseXrefs).HasForeignKey(d => d.IdSense);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
