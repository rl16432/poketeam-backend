using Microsoft.EntityFrameworkCore;

namespace msa_phase_2_backend.Models
{
    public static class SeedData
    {
        // Used to add users in the database on initialisation
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new UserContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<UserContext>>()))
            {
                // Look for any users
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                context.Users.AddRange(
                    new User
                    {
                        UserName = "Ash"
                    },

                    new User
                    {
                        UserName = "Misty"
                    },

                    new User
                    {
                        UserName = "Brock"
                    },

                    new User
                    {
                        UserName = "Cynthia"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}