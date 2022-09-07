using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Data;

namespace msa_phase_3_backend.Services
{
    // Add dummy users to the database
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-6.0&tabs=visual-studio
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
                    return; // DB has been seeded
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