using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using poketeam_backend.Domain.Models;
using poketeam_backend.Repository.Data;

namespace poketeam_backend.Repository
{
    // Add dummy users to the database
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-6.0&tabs=visual-studio
    public static class SeedData
    {
        // Used to add users in the database on initialisation
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>());
            // Look for any users
            if (context.Trainers.Any())
            {
                return; // DB has been seeded
            }

            context.Trainers.AddRange(
                new Trainer
                {
                    UserName = "Ash"
                },

                new Trainer
                {
                    UserName = "Misty"
                },

                new Trainer
                {
                    UserName = "Brock"
                },

                new Trainer
                {
                    UserName = "Cynthia"
                }
            );
            context.SaveChanges();
        }
    }
}