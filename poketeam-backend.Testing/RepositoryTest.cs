using FluentAssertions;
using poketeam_backend.Domain.Models;
using poketeam_backend.Repository.Repository;

namespace poketeam_backend.Testing
{
    [TestFixture]
    [Category("Repositories")]
    public class RepositoryTest
    {
        private DbTestSetup testSetup;
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
            testSetup = new DbTestSetup();
            pokemonRepository = new PokemonRepository(testSetup.AppContext);
            trainerRepository = new TrainerRepository(testSetup.AppContext);
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = trainerRepository.Get(userId);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo(testSetup.UserNames[0]);
        }

        [Test]
        public void GetUserByUserName_GetsCorrectUser()
        {
            string userName = testSetup.UserNames[0];
            var result = trainerRepository.GetByUserName(userName);

            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo(userName);
            result.Should().BeEquivalentTo(testSetup.AppContext.Trainers.Single(x => x.UserName == userName));
        }

        [Test]
        public void GetUserByUserName_ShouldBeCaseSensitive()
        {
            string userName = testSetup.UserNames[0].ToLower() == testSetup.UserNames[0] ? testSetup.UserNames[0].ToUpper() : testSetup.UserNames[0].ToLower();
            var result = trainerRepository.GetByUserName(userName);
            result.Should().BeNull();
        }

        [Test]
        public void GetNonExistentUsers_ReturnsNull()
        {
            string userName = "nqaoiwdoiwoiqjw";
            var result = trainerRepository.GetByUserName(userName);
            result.Should().BeNull();

            int userId = testSetup.UserNames.Count + 10;
            var result_2 = trainerRepository.Get(userId);
            result_2.Should().BeNull();
        }

        [Test]
        public void GetAllUsers_GetsAllUsers()
        {
            var result = trainerRepository.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(testSetup.UserNames.Count);
        }

        [Test]
        public void DeleteUser_RemovesUser()
        {
            var userId = 1;
            var user = testSetup.AppContext.Trainers.First(u => u.Id == userId);

            trainerRepository.Delete(user);

            testSetup.AppContext.Trainers.Count().Should().Be(testSetup.UserNames.Count - 1);
            testSetup.AppContext.Trainers.Select(x => x.Id).Should().NotContain(userId);
        }

        [Test]
        public void InsertUser_AddsUser()
        {
            var newUser = new Trainer { UserName = "qpweoqiwpeoi" };
            trainerRepository.Insert(newUser);

            testSetup.AppContext.Trainers.Count().Should().Be(testSetup.UserNames.Count + 1);
            testSetup.AppContext.Trainers.Select(x => x.Id).Should().Contain(testSetup.UserNames.Count + 1);
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            int userId = 1;
            var result = trainerRepository.Get(userId);
            var newUserName = "qpuwoim";

            result.UserName = newUserName;
            trainerRepository.Update(result);

            testSetup.AppContext.Trainers.First(u => u.Id == userId).UserName
                .Should().BeEquivalentTo(newUserName);
        }
        
    }
}