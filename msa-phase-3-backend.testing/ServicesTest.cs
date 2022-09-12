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
using System.Text.Json;

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
            var options = new DbContextOptionsBuilder<UserContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            userContext = new UserContext(options);

            userContext.Users.Add(new User { UserName = "Bianca" });
            userContext.Users.Add(new User { UserName = "Ralph" });
            userContext.Users.Add(new User { UserName = "Skyla" });
            userContext.Users.Add(new User { UserName = "Diamond" });
            userContext.SaveChanges();

            // Read test Pokemon list from JSON
            StreamReader r = new(Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestFiles", "pokemon_list.json"));

            string json = r.ReadToEnd();

            pokemonList = JsonSerializer.Deserialize<List<Pokemon>>(json)!;

            // Add first half of the list to "Diamond" user for testing purposes
            foreach (Pokemon pokemon in pokemonList!.GetRange(0, pokemonList.Count / 2))
            {
                var user = userContext.Users.FirstOrDefault(x => x.UserName.Equals("Diamond"));

                userContext.Pokemon.Add(pokemon);
                user!.Pokemon!.Add(pokemon);
                userContext.Users.Update(user);
            }

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
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Bianca");
        }
        [Test]
        public void GetUserByUserName_GetsCorrectUser()
        {
            string userName = "Skyla";
            var result = userService.GetByUserName(userName);
            result.Should().NotBeNull();
            result.UserName.Should().BeEquivalentTo("Skyla");
        }
        [Test]
        public void GetUserByUserName_ShouldBeCaseSensitive()
        {
            string userName = "skyla";
            var result = userService.GetByUserName(userName);
            result.Should().BeNull();
        }
        [Test]
        public void GetNonExistentUser_ReturnsNull()
        {
            string userName = "Jack";
            var result = userService.GetByUserName(userName);
            result.Should().BeNull();

            int userId = 20;
            var result_2 = userService.Get(userId);
            result_2.Should().BeNull();
        }

        [Test]
        public void GetAllUsers_GetsAllUsers()
        {
            var result = userService.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(4);
        }

        [Test]
        public void DeleteById_RemovesUser()
        {
            var userId = 2;
            userService.DeleteById(userId);

            var result = userService.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            // Check that the user has been removed
            result.ToList().Select(x => x.Id).Should().NotContain(2);
        }

        [Test]
        public void InsertUser_AddsUser()
        {
            var newUser = new User { UserName = "Jack" };
            userService.Insert(newUser);

            var result = userService.GetAll();

            result.Should().NotBeNull();
            result.Count().Should().Be(5);
            result.ToList().Select(x => x.Id).Should().Contain(5);
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            int userId = 1;
            var result = userService.Get(userId);

            result.UserName = "Cheren";
            userService.Update(result);

            var result_2 = userService.Get(userId);

            result_2.Should().NotBeNull();
            result_2.UserName.Should().BeEquivalentTo("Cheren");
        }

        [Test]
        public void DeletePokemonFromUser_DeletesPokemon()
        {
            var originalCount = userContext.Pokemon.Count();
            var pokemon = userContext.Pokemon.FirstOrDefault(p => p.Name!.ToLower().Equals("celebi"));

            pokemon.Should().NotBeNull();

            pokemonService.Delete(pokemon!);

            userContext.Pokemon.Count().Should().Be(originalCount - 1);
            userContext.Pokemon.Should().NotContain(pokemon!);
        }

        [Test]
        public void AddPokemonToUser_AddsToUser()
        {
            var originalCount = userContext.Pokemon.Count();
            var pokemon = pokemonList.First(p => p.Name!.Equals("Primarina"));

            var user = userContext.Users.First(user => user.UserName!.Equals("Ralph"));
            var userPokemonCount = user.Pokemon!.Count;

            pokemon.Should().NotBeNull();

            // Add Pokemon to database first
            pokemonService.Insert(pokemon);
            
            userContext.Pokemon.Count().Should().Be(originalCount + 1);
            userContext.Pokemon.Should().Contain(pokemon);
            
            // Add Pokemon to user collection
            user.Pokemon!.Add(pokemon);
            userService.Update(user);

            userContext.Users.First(user => user.UserName!.Equals("Ralph")).Pokemon!.Count.Should().Be(userPokemonCount + 1);
        }
    }
}