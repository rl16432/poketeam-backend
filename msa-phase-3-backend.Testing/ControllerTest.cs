using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Domain.Models.DTO;
using msa_phase_3_backend.API.Controllers;
using Moq;
using NSubstitute;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using msa_phase_3_backend.Repository.Data;
using Microsoft.Extensions.Configuration;
using msa_phase_3_backend.Repository.Repository;
using msa_phase_3_backend.Services.CustomServices;
using FluentAssertions;
using System.Text.Json;

namespace msa_phase_3_backend.testing
{
    [TestFixture]
    public class ControllerTest
    {
        private IHttpClientFactory httpClientFactoryMock;
        private UserController controller;
        private UserValidator userValidator;
        private UserContext userContext;
        private UserServices userService;
        private PokemonServices pokemonService;
        private IConfiguration configuration;
        private List<Pokemon> pokemonList;
        private User userWithPokemon;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Read test Pokemon schema from JSON
            StreamReader r = new(Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestFiles", "litten.json"));

            string json = r.ReadToEnd();

            // Substitute PokeApi HttpClient, which always returns "litten.json"
            // https://anthonygiretti.com/2018/09/06/how-to-unit-test-a-class-that-consumes-an-httpclient-with-ihttpclientfactory-in-asp-net-core/
            httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                // Return the litten.json stringified
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                // Set base address to avoid invalid Uri
                BaseAddress = new Uri("https://pokeapi.co/api/v2")
            };

            httpClientFactoryMock.CreateClient("pokeapi").Returns(fakeHttpClient);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            var configurationMock = new Mock<IConfiguration>();

            configurationSectionMock
               .Setup(x => x.Value)
               .Returns("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork");

            configurationMock
               .Setup(x => x.GetSection("PokemonArtworkAddress"))
               .Returns(configurationSectionMock.Object);

            configuration = configurationMock.Object;

            // Initialise FluentValiator
            userValidator = new UserValidator();


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

            var pokemonRepository = new PokemonRepository(userContext);
            var userRepository = new UserRepository(userContext);

            userService = new UserServices(userRepository, pokemonRepository);
            pokemonService = new PokemonServices(pokemonRepository);

            // Read test Pokemon list from JSON
            StreamReader r = new(Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestFiles", "pokemon_list.json"));

            string json = r.ReadToEnd();

            pokemonList = JsonSerializer.Deserialize<List<Pokemon>>(json)!;

            // User which we add Pokemon to for testing purposes
            userWithPokemon = userContext.Users.FirstOrDefault(x => x.UserName.Equals("Diamond"))!;

            // Add first half of the list to "Diamond" user for testing purposes
            foreach (Pokemon pokemon in pokemonList!.GetRange(0, pokemonList.Count / 2))
            {
                userContext.Pokemon.Add(pokemon);
                userWithPokemon!.Pokemon.Add(pokemon);
                userContext.Users.Update(userWithPokemon);
            }

            userContext.SaveChanges();

            // Set up mock logger
            var mockLogger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
            }).CreateLogger<UserController>();

            controller = new UserController(mockLogger, httpClientFactoryMock,
                configuration, userService, pokemonService, userValidator);
        }

        [Test]
        public void GetUserByName_ReturnsOkObjectResult()
        {
            string userName = "Skyla";
            var result = controller.GetUser(userName);

            // Test that result is OkObjectResult
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }

        // Test that OkObjectResult is returned by UserController.GetUsers()
        [Test]
        public void GetAllUsers_ReturnsOkObjectResult()
        {
            var result = controller.GetUsers();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetAllUsers_ReturnsAllUsers()
        {
            var result = controller.GetUsers();
            var typedResult = result.Result as OkObjectResult;

            typedResult!.Value.Should().NotBeNull();
            typedResult.Value.Should().BeAssignableTo<IEnumerable<User>>();

            var typedValue = typedResult.Value as IEnumerable<User>;

            // The initial database should have 3 users
            typedValue!.Count().Should().Be(4);
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedAtActionResult()
        {
            var result = await controller.PostUser(new UserCreateDTO { UserName = "Volkner" });

            result.Result.Should().BeAssignableTo<CreatedAtActionResult>();
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedItem()
        {
            var result = await controller.PostUser(new UserCreateDTO { UserName = "Cheren" });

            var typedResult = result.Result as CreatedAtActionResult;
            var value = typedResult!.Value as User;

            // UserName is correct
            value!.UserName.Should().BeEquivalentTo("Cheren");

            // Pokemon List is initialised as empty
            value!.Pokemon.Should().BeAssignableTo<List<Pokemon>>();
            value!.Pokemon.Should().BeEmpty();
            // UserId is some integer
            value!.Id.Should().BeOfType(typeof(int));
        }

        [Test]
        public async Task CreateDuplicateUser_IsNotAllowed()
        {
            int originalCount = userContext.Users.Count();
            var result = await controller.PostUser(new UserCreateDTO { UserName = "Cheren" });

            var result_2 = await controller.PostUser(new UserCreateDTO { UserName = "Cheren" });

            // Check ConflictResult is returned
            result_2.Result.Should().BeAssignableTo<ConflictObjectResult>();
            // Check the number of Users only increases by 1
            userContext.Users.Count().Should().Be(originalCount + 1);
        }

        [Test]
        public async Task AddPokemonToUser_ReturnsNoContentResult()
        {
            var result = await controller.AddPokemonToUser(2, "litten");

            result.Result.Should().BeAssignableTo<NoContentResult>();
        }

        [Test]
        public async Task AddPokemonToUser_AddsPokemonToUser()
        {
            int userId = 2;

            var result = await controller.AddPokemonToUser(userId, "litten");

            // Get user after Pokemon is added
            var updatedUser = await userContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            updatedUser.Should().NotBeNull();
            // Count of Pokemon under user should be 1
            updatedUser!.Pokemon.Count.Should().Be(1);
            updatedUser.Pokemon.First().Name.Should().BeEquivalentTo("Litten");
        }

        [Test]
        public async Task AddPokemonToUser_DoesNotAddDuplicate()
        {
            int userId = 2;

            await controller.AddPokemonToUser(userId, "litten");

            // Attempt to add second litten
            var result = await controller.AddPokemonToUser(userId, "litten");

            // Get user after Pokemon is added
            var updatedUser = await userContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            updatedUser.Should().NotBeNull();
            // Count of Pokemon under user should be 1
            updatedUser!.Pokemon.Count.Should().Be(1);
            updatedUser.Pokemon.First().Name.Should().BeEquivalentTo("Litten");
        }

        public async Task AddPokemonToUser_LimitsToSixPokemon()
        {
            int userId = 1;
            var userToAdd = userContext.Users.First(u => u.Id == userId);

            foreach (Pokemon pokemon in pokemonList.GetRange(0, pokemonList.Count / 2))
            {
                userContext.Pokemon.Add(pokemon);
                userToAdd.Pokemon.Add(pokemon);
                userContext.Users.Update(userToAdd);
            }

            await userContext.SaveChangesAsync();

            var result = await controller.AddPokemonToUser(userId, "litten");

            User updatedUser = userContext.Users.First(u => u.Id == userId);

            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
            updatedUser.Pokemon.Count.Should().Be(6);
        }

        [Test]
        public void DeletePokemonFromUser_ReturnsOkResult()
        {
            var result = controller.DeletePokemonFromUser(userWithPokemon.Id, pokemonList[1].Name!);

            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public void DeletePokemonFromUser_RemovesCorrectPokemon()
        {
            var originalPokemonCount = userWithPokemon.Pokemon.Count;
            var result = controller.DeletePokemonFromUser(userWithPokemon.Id, pokemonList[1].Name!);

            var newUserWithPokemon = userContext.Users.First(u => u.Id == userWithPokemon.Id);

            newUserWithPokemon.Pokemon.Count.Should().Be(originalPokemonCount - 1);
            newUserWithPokemon.Pokemon.Select(x => x.Name).Should().NotContainEquivalentOf(pokemonList[1].Name);
        }

        [Test]
        public void DeletePokemonFromInvalidUser_ReturnsNotFoundResult()
        {
            var result = controller.DeletePokemonFromUser(20, pokemonList[1].Name!);

            result.Should().BeAssignableTo<NotFoundObjectResult>();
        }
    }
}