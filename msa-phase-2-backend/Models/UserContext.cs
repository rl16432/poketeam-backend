using Microsoft.EntityFrameworkCore;

namespace msa_phase_2_backend.Models;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Pokemon> Pokemon { get; set; } = default!;

}