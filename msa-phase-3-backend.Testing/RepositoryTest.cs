using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Domain.Models.DTO;
using msa_phase_3_backend.API.Controllers;
using NSubstitute;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Services.CustomServices;
using msa_phase_3_backend.Repository.Repository;

namespace msa_phase_3_backend.testing
{
    [TestFixture]
    [Category("Repositories")]
    public class RepositoryTest
    {
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
            userContext.Users.Add(new User { UserName = "Diamond" });
            userContext.SaveChanges();

            pokemonRepository = new PokemonRepository(userContext);
            userRepository = new UserRepository(userContext);
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = userRepository.Get(userId);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Bianca");
        }

        [Test]
        public void GetUserByUserName_GetsCorrectUser()
        {
            string userName = "Skyla";
            var result = userRepository.GetByUserName(userName);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Skyla");
        }

        [Test]
        public void GetUserByUserName_ShouldBeCaseSensitive()
        {
            string userName = "skyla";
            var result = userRepository.GetByUserName(userName);
            result.Should().BeNull();
        }

        [Test]
        public void GetNonExistentUsers_ReturnsNull()
        {
            string userName = "Jack";
            var result = userRepository.GetByUserName(userName);
            result.Should().BeNull();

            int userId = 20;
            var result_2 = userRepository.Get(userId);
            result_2.Should().BeNull();
        }

        [Test]
        public void GetAllUsers_GetsAllUsers()
        {
            var result = userRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(4);
        }

        [Test]
        public void DeleteById_RemovesUser()
        {
            var userId = 2;
            userRepository.DeleteById(userId);

            var result = userRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            // Check that the user has been removed
            result.ToList().Select(x => x.Id).Should().NotContain(2);
        }

        [Test]
        public void InsertUser_AddsUser()
        {
            var newUser = new User { UserName = "Jack" };
            userRepository.Insert(newUser);

            var result = userRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(5);
            result.ToList().Select(x => x.Id).Should().Contain(5);
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            int userId = 1;
            var result = userRepository.Get(userId);

            result.UserName = "Cheren";
            userRepository.Update(result);

            var result_2 = userRepository.Get(userId);

            result_2.Should().NotBeNull();
            result_2.UserName.Should().BeEquivalentTo("Cheren");
        }
    }
}