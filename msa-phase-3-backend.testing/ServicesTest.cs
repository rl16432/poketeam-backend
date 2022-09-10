using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Domain.Models.DTO;
using msa_phase_3_backend.API.Controllers;
using NSubstitute;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Services.CustomServices;
using msa_phase_3_backend.Repository.Repository;

namespace msa_phase_3_backend.testing
{
    [TestFixture]
    [Category("UserServices")]
    public class ServicesTests
    {
        private UserServices userService;
        private PokemonServices pokemonService;
        private UserContext userContext;
        private PokemonRepository pokemonRepository;
        private UserRepository userRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            
        }

        // Set up after every test
        [SetUp]
        public void Setup()
        {
            // Use EF Core in memory database for testing. New database on every test
            // Guid is virtually unique
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            userContext = new UserContext(options);

            userContext.Users.Add(new User { UserName = "Bianca" });
            userContext.Users.Add(new User { UserName = "Ralph" });
            userContext.Users.Add(new User { UserName = "Skyla" });
            userContext.SaveChanges();

            pokemonRepository = new PokemonRepository(userContext);
            userRepository = new UserRepository(userContext);

            userService = new UserServices(userRepository, pokemonRepository);
            pokemonService = new PokemonServices(pokemonRepository);
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = userService.Get(userId);
            Assert.NotNull(result);
            Assert.That(result.UserName, Is.EqualTo("Bianca"));
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = userService.Get(userId);
            Assert.NotNull(result);
            Assert.That(result.UserName, Is.EqualTo("Bianca"));

        }

    }
}