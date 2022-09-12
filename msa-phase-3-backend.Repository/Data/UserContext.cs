using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Domain.Models;

namespace msa_phase_3_backend.Repository.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().Property(u => u.UserName)
            .UseCollation("SQL_Latin1_General_CP1_CS_AS");

        builder.Entity<Pokemon>().Property(a => a.Name)
           .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        builder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }

    public virtual DbSet<User> Users { get; set; } = default!;

    public virtual DbSet<Pokemon> Pokemon { get; set; } = default!;

}