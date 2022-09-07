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
    }

    public virtual DbSet<User> Users { get; set; } = default!;

    public virtual DbSet<Pokemon> Pokemon { get; set; } = default!;

}