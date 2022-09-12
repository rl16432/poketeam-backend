using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using msa_phase_3_backend.API.Controllers;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Domain.Models.DTO;
using msa_phase_3_backend.Repository.Repository;
using msa_phase_3_backend.Services.CustomServices;
using NSubstitute;
using System.Net;
using System.Text;

namespace msa_phase_3_backend.Testing
{
    [TestFixture]
    public class ControllerTest
    {
        private IHttpClientFactory httpClientFactoryMock;
        private TrainerValidator trainerValidator;
        private IConfiguration configuration;
        private DbTestSetup testSetup;
        private TrainerController controller;

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
            trainerValidator = new TrainerValidator();
        }

        // Set up after every test
        [SetUp]
        public void Setup()
        {
            testSetup = new DbTestSetup(); 
            var pokemonRepository = new PokemonRepository(testSetup.AppContext);
            var userRepository = new TrainerRepository(testSetup.AppContext);

            var trainerService = new TrainerServices(userRepository, pokemonRepository);
            var pokemonService = new PokemonServices(pokemonRepository);

            // Set up mock logger
            var mockLogger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
            }).CreateLogger<TrainerController>();

            controller = new TrainerController(mockLogger, httpClientFactoryMock,
                configuration, trainerService, pokemonService, trainerValidator);
        }

        [Test]
        public void GetUserByName_ReturnsOkObjectResult()
        {
            string userName = testSetup.UserNames[1];
            var result = controller.GetUser(userName);

            // Test that result is OkObjectResult
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }

        // Test that OkObjectResult is returned by UserController.GetUsers()
        [Test]
        public void GetAllUsers_ReturnsOkObjectResult()
        {
            var result = controller.GetUsers();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }

        [Test]
        public void GetAllUsers_ReturnsAllUsers()
        {
            var result = controller.GetUsers();

            var typedResult = result.Result as OkObjectResult;
            var typedValue = typedResult!.Value as IEnumerable<Trainer>;

            // Test the number of users in the database is correct
            typedValue!.Count().Should().Be(testSetup.UserNames.Count);
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedAtActionResult()
        {
            var result = await controller.PostUser(new UserCreateDTO { UserName = "qwekjqwlk" });

            result.Result.Should().BeAssignableTo<CreatedAtActionResult>();
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedItem()
        {
            var newUserName = "qlewkjq";
            var result = await controller.PostUser(new UserCreateDTO { UserName = newUserName });

            var typedResult = result.Result as CreatedAtActionResult;
            var value = typedResult!.Value as Trainer;

            // UserName is correct
            value!.UserName.Should().BeEquivalentTo(newUserName);

            // Pokemon List is initialised as empty
            value!.Pokemon.Should().BeAssignableTo<List<Pokemon>>();
            value!.Pokemon.Should().BeEmpty();

            // UserId is some integer
            value!.Id.Should().BeOfType(typeof(int));
        }

        [Test]
        public async Task CreateUser_ShouldBeCaseSensitive()
        {
            var userName = testSetup.UserNames[0].ToLower() == testSetup.UserNames[0] ? testSetup.UserNames[0].ToUpper() : testSetup.UserNames[0].ToLower();
            var result = await controller.PostUser(new UserCreateDTO { UserName = userName });

            result.Result.Should().NotBeAssignableTo<ConflictObjectResult>();

            var typedResult = result.Result as CreatedAtActionResult;
            var value = typedResult!.Value as Trainer;

            // UserName is correct
            value!.UserName.Should().BeEquivalentTo(userName);

            // Check that there is only one match for the new username
            testSetup.AppContext.Trainers.Where(u => u.UserName == userName).Count().Should().Be(1);
        }
        [Test]
        public async Task CreateDuplicateUser_IsNotAllowed()
        {
            int originalCount = testSetup.AppContext.Trainers.Count();
            var result = await controller.PostUser(new UserCreateDTO { UserName = "qweoiqjw" });

            var result_2 = await controller.PostUser(new UserCreateDTO { UserName = "qweoiqjw" });

            // Check ConflictResult is returned
            result_2.Result.Should().BeAssignableTo<ConflictObjectResult>();
            // Check the number of Users only increases by 1
            testSetup.AppContext.Trainers.Count().Should().Be(originalCount + 1);
        }

        [Test]
        public async Task AddPokemonToUser_ReturnsNoContentResult()
        {
            var result = await controller.AddPokemonToUser(2, "litten");

            result.Result.Should().BeAssignableTo<NoContentResult>();
        }

        [Test]
        public async Task AddPokemonToUser_AddsPokemonToUserCollection()
        {
            int userId = 1;

            var result = await controller.AddPokemonToUser(userId, "litten");

            // Get user after Pokemon is added
            var updatedUser = await testSetup.AppContext.Trainers.FirstOrDefaultAsync(u => u.Id == userId);

            updatedUser.Should().NotBeNull();

            // Check count 
            updatedUser!.Pokemon.Count.Should().Be(1);
            updatedUser.Pokemon.First().Name.Should().BeEquivalentTo("Litten");
        }

        [Test]
        public async Task AddPokemonToUser_DoesNotAddDuplicate()
        {
            int userId = 1;

            await controller.AddPokemonToUser(userId, "litten");

            // Attempt to add second litten
            var result = await controller.AddPokemonToUser(userId, "litten");

            // Get user after Pokemon is added
            var updatedUser = await testSetup.AppContext.Trainers.FirstOrDefaultAsync(u => u.Id == userId);

            updatedUser.Should().NotBeNull();
            // Count of Pokemon under user should be 1
            updatedUser!.Pokemon.Count.Should().Be(1);
            updatedUser.Pokemon.First().Name.Should().BeEquivalentTo("Litten");
        }
        [Test]
        public async Task AddPokemonToUser_LimitsToSixPokemon()
        {
            int userId = 1;
            var userToAdd = testSetup.AppContext.Trainers.First(u => u.Id == userId);

            // Read Pokemon from JSON

            var pokemonList = testSetup.getPokemonListFromJson("pokemon_list.json");
            // Add 6 Pokemon to the user with ID: 1
            foreach (Pokemon pokemon in pokemonList.GetRange(0, 6))
            {
                testSetup.AppContext.Pokemon.Add(pokemon);
                userToAdd.Pokemon.Add(pokemon);
                testSetup.AppContext.Trainers.Update(userToAdd);
            }
            await testSetup.AppContext.SaveChangesAsync();

            // Attempt to add another
            var result = await controller.AddPokemonToUser(userId, "litten");

            Trainer updatedUser = testSetup.AppContext.Trainers.First(u => u.Id == userId);
            
            // Check there is a bad request
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
            // Check the number of pokemon is still 6
            updatedUser.Pokemon.Count.Should().Be(6);
        }

        [Test]
        public void DeletePokemonFromUser_ReturnsOkResult()
        {
            var result = controller.DeletePokemonFromUser(testSetup.UserWithPokemon.Id, testSetup.AddedPokemonList[0].Name);

            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public void DeletePokemonFromUser_RemovesCorrectPokemon()
        {
            var originalPokemonCount = testSetup.UserWithPokemon.Pokemon.Count;
            var result = controller.DeletePokemonFromUser(testSetup.UserWithPokemon.Id, testSetup.AddedPokemonList[1].Name);

            var newUserWithPokemon = testSetup.AppContext.Trainers.First(u => u.Id == testSetup.UserWithPokemon.Id);

            newUserWithPokemon.Pokemon.Count.Should().Be(originalPokemonCount - 1);
            newUserWithPokemon.Pokemon.Select(x => x.Name).Should().NotContainEquivalentOf(testSetup.AddedPokemonList[1].Name);
        }

        [Test]
        public void DeletePokemonFromInvalidUser_ReturnsNotFoundResult()
        {
            var result = controller.DeletePokemonFromUser(20, testSetup.AddedPokemonList[0].Name);

            result.Should().BeAssignableTo<NotFoundObjectResult>();
        }
    }
}