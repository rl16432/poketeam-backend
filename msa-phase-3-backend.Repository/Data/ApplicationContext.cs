using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Domain.Models;

namespace msa_phase_3_backend.Repository.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Trainer>().Property(u => u.UserName)
            .UseCollation("SQL_Latin1_General_CP1_CS_AS");

        builder.Entity<Pokemon>().Property(a => a.Name)
           .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        builder.Entity<Trainer>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }

    public virtual DbSet<Trainer> Trainers { get; set; } = default!;

    public virtual DbSet<Pokemon> Pokemon { get; set; } = default!;

}