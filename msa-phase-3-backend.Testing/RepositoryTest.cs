using msa_phase_3_backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Repository.Repository;

namespace msa_phase_3_backend.testing
{
    [TestFixture]
    [Category("Repositories")]
    public class RepositoryTest
    {
        private ApplicationDbContext appContext;
        private PokemonRepository pokemonRepository;
        private TrainerRepository trainerRepository;

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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            appContext = new ApplicationDbContext(options);

            appContext.Trainers.Add(new Trainer { UserName = "Bianca" });
            appContext.Trainers.Add(new Trainer { UserName = "Ralph" });
            appContext.Trainers.Add(new Trainer { UserName = "Skyla" });
            appContext.Trainers.Add(new Trainer { UserName = "Diamond" });
            appContext.SaveChanges();

            pokemonRepository = new PokemonRepository(appContext);
            trainerRepository = new TrainerRepository(appContext);
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = trainerRepository.Get(userId);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Bianca");
        }

        [Test]
        public void GetUserByUserName_GetsCorrectUser()
        {
            string userName = "Skyla";
            var result = trainerRepository.GetByUserName(userName);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Skyla");
        }

        [Test]
        public void GetUserByUserName_ShouldBeCaseSensitive()
        {
            string userName = "skyla";
            var result = trainerRepository.GetByUserName(userName);
            result.Should().BeNull();
        }

        [Test]
        public void GetNonExistentUsers_ReturnsNull()
        {
            string userName = "Jack";
            var result = trainerRepository.GetByUserName(userName);
            result.Should().BeNull();

            int userId = 20;
            var result_2 = trainerRepository.Get(userId);
            result_2.Should().BeNull();
        }

        [Test]
        public void GetAllUsers_GetsAllUsers()
        {
            var result = trainerRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(4);
        }

        [Test]
        public void DeleteById_RemovesUser()
        {
            var userId = 2;
            var user = trainerRepository.Get(userId);

            trainerRepository.Delete(user);

            var result = trainerRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            // Check that the user has been removed
            result.ToList().Select(x => x.Id).Should().NotContain(2);
        }

        [Test]
        public void InsertUser_AddsUser()
        {
            var newUser = new Trainer { UserName = "Jack" };
            trainerRepository.Insert(newUser);

            var result = trainerRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(5);
            result.ToList().Select(x => x.Id).Should().Contain(5);
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            int userId = 1;
            var result = trainerRepository.Get(userId);

            result.UserName = "Cheren";
            trainerRepository.Update(result);

            var result_2 = trainerRepository.Get(userId);

            result_2.Should().NotBeNull();
            result_2.UserName.Should().BeEquivalentTo("Cheren");
        }
    }
}