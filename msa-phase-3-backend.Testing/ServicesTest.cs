using msa_phase_3_backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Services.CustomServices;
using msa_phase_3_backend.Repository.Repository;
using System.Text.Json;

namespace msa_phase_3_backend.Testing
{
    [TestFixture]
    [Category("TrainerServices")]
    public class ServicesTests
    {
        private TrainerServices trainerService;
        private PokemonServices pokemonService;
        private DbTestSetup testSetup;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        // Set up after every test
        [SetUp]
        public void Setup()
        {
            testSetup = new DbTestSetup();

            PokemonRepository pokemonRepository = new PokemonRepository(testSetup.AppContext);
            TrainerRepository trainerRepository = new TrainerRepository(testSetup.AppContext);

            trainerService = new TrainerServices(trainerRepository, pokemonRepository);
            pokemonService = new PokemonServices(pokemonRepository);
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = trainerService.Get(userId);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo(testSetup.UserNames[0]);
        }

        [Test]
        public void GetUserByUserName_GetsCorrectUser()
        {
            string userName = testSetup.UserNames[1];
            var result = trainerService.GetByUserName(userName);

            result.UserName.Should().BeEquivalentTo(testSetup.UserNames[1]);
            result.Should().BeEquivalentTo(testSetup.AppContext.Trainers.Single(x => x.UserName == userName));
        }

        [Test]
        public void GetUserByUserName_ShouldBeCaseSensitive()
        {
            string userName = testSetup.UserNames[0].ToLower() == testSetup.UserNames[0] ? testSetup.UserNames[0].ToUpper() : testSetup.UserNames[0].ToLower();
            var result = trainerService.GetByUserName(userName);
            result.Should().BeNull();
        }

        [Test]
        public void GetNonExistentUser_ReturnsNull()
        {
            string userName = "nqaoiwdoiwoiqjw";
            var result = trainerService.GetByUserName(userName);
            result.Should().BeNull();

            int userId = testSetup.UserNames.Count + 10;
            var result_2 = trainerService.Get(userId);
            result_2.Should().BeNull();
        }

        [Test]
        public void GetAllUsers_GetsAllUsers()
        {
            var result = trainerService.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(testSetup.UserNames.Count);
        }

        [Test]
        public void DeleteById_RemovesUser()
        {
            var userId = 1;

            trainerService.DeleteById(userId);

            // Check the count is correct
            testSetup.AppContext.Trainers.Count().Should().Be(testSetup.UserNames.Count - 1);
            // Check that the user has been removed
            testSetup.AppContext.Trainers.Select(x => x.Id).Should().NotContain(userId);
        }

        [Test]
        public void InsertUser_AddsUser()
        {
            var newUser = new Trainer { UserName = "Jack" };
            trainerService.Insert(newUser);

            var result = trainerService.GetAll();

            // Check count is correct
            testSetup.AppContext.Trainers.Count().Should().Be(testSetup.UserNames.Count + 1);
            // Check the ID is incremented
            testSetup.AppContext.Trainers.Select(x => x.Id).Should().Contain(testSetup.UserNames.Count + 1);
            testSetup.AppContext.Trainers.Select(x => x.UserName).Should().ContainEquivalentOf(newUser.UserName);
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            int userId = 1;
            var result = trainerService.Get(userId);
            var newUserName = "qpuwoim";

            result.UserName = newUserName;
            trainerService.Update(result);

            testSetup.AppContext.Trainers.First(u => u.Id == userId).Should().NotBeNull();
            testSetup.AppContext.Trainers.First(u => u.Id == userId).UserName.Should().BeEquivalentTo(newUserName);
        }

        [Test]
        public void DeletePokemonFromUser_DeletesPokemon()
        {
            var originalCount = testSetup.AppContext.Pokemon.Count();
            var pokemon = testSetup.AddedPokemonList[0];

            pokemonService.Delete(pokemon);

            testSetup.AppContext.Pokemon.Count().Should().Be(originalCount - 1);
            testSetup.AppContext.Pokemon.Should().NotContain(pokemon);

            // Check that the user which has the Pokemon no longer has it in their list
            testSetup.AppContext.Trainers.First(u => u.Id == testSetup.UserWithPokemon.Id).Pokemon.Count
                .Should().Be(testSetup.AddedPokemonList.Count - 1);
        }

        [Test]
        public void AddPokemonToUser_AddsToUser()
        {
            var originalCount = testSetup.AppContext.Pokemon.Count();
            var pokemon = testSetup.NotAddedPokemonList.First();

            var userToAddTo = testSetup.AppContext.Trainers.First();
            var userPokemonCount = userToAddTo.Pokemon.Count;

            // Add Pokemon to database first
            pokemonService.Insert(pokemon);

            // Add Pokemon to user collection
            userToAddTo.Pokemon!.Add(pokemon);
            trainerService.Update(userToAddTo);

            pokemon.Should().NotBeNull();

            // Check the Pokemon DbSet contains the new pokemon
            testSetup.AppContext.Pokemon.Count().Should().Be(originalCount + 1);
            testSetup.AppContext.Pokemon.Should().Contain(pokemon);


            testSetup.AppContext.Trainers.First(user => user.UserName.Equals(userToAddTo.UserName)).Pokemon.Count
                .Should().Be(userPokemonCount + 1);
        }
    }
}