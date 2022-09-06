using Microsoft.EntityFrameworkCore;

namespace msa_phase_3_backend.Models;

public class UserContext : DbContext
{
    public UserContext()
    {

    }
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } = default!;

    public virtual DbSet<Pokemon> Pokemon { get; set; } = default!;

}