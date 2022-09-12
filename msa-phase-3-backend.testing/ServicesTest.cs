using msa_phase_3_backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Services.CustomServices;
using msa_phase_3_backend.Repository.Repository;
using System.Text.Json;

namespace msa_phase_3_backend.testing
{
    [TestFixture]
    [Category("TrainerServices")]
    public class ServicesTests
    {
        private TrainerServices trainerService;
        private PokemonServices pokemonService;
        private ApplicationDbContext appContext;
        private PokemonRepository pokemonRepository;
        private TrainerRepository trainerRepository;
        private List<Pokemon> pokemonList;

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
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            appContext = new ApplicationDbContext(options);

            appContext.Trainers.Add(new Trainer { UserName = "Bianca" });
            appContext.Trainers.Add(new Trainer { UserName = "Ralph" });
            appContext.Trainers.Add(new Trainer { UserName = "Skyla" });
            appContext.Trainers.Add(new Trainer { UserName = "Diamond" });
            appContext.SaveChanges();

            // Read test Pokemon list from JSON
            StreamReader r = new(Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestFiles", "pokemon_list.json"));

            string json = r.ReadToEnd();

            pokemonList = JsonSerializer.Deserialize<List<Pokemon>>(json)!;

            // Add first half of the list to "Diamond" user for testing purposes
            foreach (Pokemon pokemon in pokemonList!.GetRange(0, pokemonList.Count / 2))
            {
                var user = appContext.Trainers.FirstOrDefault(x => x.UserName.Equals("Diamond"));

                appContext.Pokemon.Add(pokemon);
                user!.Pokemon!.Add(pokemon);
                appContext.Trainers.Update(user);
            }

            appContext.SaveChanges();

            pokemonRepository = new PokemonRepository(appContext);
            trainerRepository = new TrainerRepository(appContext);

            trainerService = new TrainerServices(trainerRepository, pokemonRepository);
            pokemonService = new PokemonServices(pokemonRepository);
        }

        [Test]
        public void GetUserById_GetsCorrectUserName()
        {
            int userId = 1;
            var result = trainerService.Get(userId);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Bianca");
        }
        [Test]
        public void GetUserByUserName_GetsCorrectUser()
        {
            string userName = "Skyla";
            var result = trainerService.GetByUserName(userName);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Skyla");
        }
        [Test]
        public void GetUserByUserName_ShouldBeCaseSensitive()
        {
            string userName = "skyla";
            var result = trainerService.GetByUserName(userName);
            result.Should().BeNull();
        }
        [Test]
        public void GetNonExistentUser_ReturnsNull()
        {
            string userName = "Jack";
            var result = trainerService.GetByUserName(userName);
            result.Should().BeNull();

            int userId = 20;
            var result_2 = trainerService.Get(userId);
            result_2.Should().BeNull();
        }

        [Test]
        public void GetAllUsers_GetsAllUsers()
        {
            var result = trainerService.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(4);
        }

        [Test]
        public void DeleteById_RemovesUser()
        {
            var userId = 2;

            trainerService.DeleteById(userId);

            var result = trainerService.GetAll();

            result.Count().Should().Be(3);
            // Check that the user has been removed
            result.ToList().Select(x => x.Id).Should().NotContain(2);
        }

        [Test]
        public void InsertUser_AddsUser()
        {
            var newUser = new Trainer { UserName = "Jack" };
            trainerService.Insert(newUser);

            var result = trainerService.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(5);
            result.ToList().Select(x => x.Id).Should().Contain(5);
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            int userId = 1;
            var result = trainerService.Get(userId);

            result.UserName = "Cheren";
            trainerService.Update(result);

            var result_2 = trainerService.Get(userId);

            result_2.Should().NotBeNull();
            result_2.UserName.Should().BeEquivalentTo("Cheren");
        }

        [Test]
        public void DeletePokemonFromUser_DeletesPokemon()
        {
            var originalCount = appContext.Pokemon.Count();
            var pokemon = appContext.Pokemon.FirstOrDefault(p => p.Name!.ToLower().Equals("celebi"));

            pokemon.Should().NotBeNull();

            pokemonService.Delete(pokemon!);

            appContext.Pokemon.Count().Should().Be(originalCount - 1);
            appContext.Pokemon.Should().NotContain(pokemon!);
        }

        [Test]
        public void AddPokemonToUser_AddsToUser()
        {
            var originalCount = appContext.Pokemon.Count();
            var pokemon = pokemonList.First(p => p.Name!.Equals("Primarina"));

            var user = appContext.Trainers.First(user => user.UserName!.Equals("Ralph"));
            var userPokemonCount = user.Pokemon!.Count;

            pokemon.Should().NotBeNull();

            // Add Pokemon to database first
            pokemonService.Insert(pokemon);
            
            appContext.Pokemon.Count().Should().Be(originalCount + 1);
            appContext.Pokemon.Should().Contain(pokemon);
            
            // Add Pokemon to user collection
            user.Pokemon!.Add(pokemon);
            trainerService.Update(user);

            appContext.Trainers.First(user => user.UserName!.Equals("Ralph")).Pokemon!.Count.Should().Be(userPokemonCount + 1);
        }
    }
}