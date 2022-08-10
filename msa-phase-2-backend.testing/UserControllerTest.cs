using msa_phase_2_backend.Models;
using msa_phase_2_backend.Models.DTO;
using msa_phase_2_backend.Controllers;
using System.Text.Json;
using NSubstitute;
using System.Net;
using System.Text;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace msa_phase_2_backend.testing
{
    public class Tests
    {
        private IHttpClientFactory httpClientFactoryMock;
        private UserController controller;

        [SetUp]
        public void Setup()
        {
            // Read test Pokemon schema from JSON
            StreamReader r = new("./litten.json");
            string json = r.ReadToEnd();
            var pokemon = JsonSerializer.Deserialize<PokeApi>(json);

            // Substitute PokeApi HttpClient
            // https://anthonygiretti.com/2018/09/06/how-to-unit-test-a-class-that-consumes-an-httpclient-with-ihttpclientfactory-in-asp-net-core/
            httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            httpClientFactoryMock.CreateClient("pokeapi").Returns(fakeHttpClient);

            // Set up mock context
            var userMockSet = new Mock<DbSet<User>>();
            var pokemonMockSet = new Mock<DbSet<Pokemon>>();

            var mockContext = new Mock<UserContext>();

            // Users attribute returns DbSet<User>
            mockContext.Setup(m => m.Users).Returns(userMockSet.Object);
            mockContext.Setup(m => m.Pokemon).Returns(pokemonMockSet.Object);

            // Set up mock logger
            var mockLogger = new Mock<ILogger<UserController>>();
            controller = new UserController(mockContext.Object, mockLogger.Object, httpClientFactoryMock);
        }
        [Test]
        public async Task CreateUser_ReturnsCreatedAtActionResult()
        {   
            var result = await controller.PostUser(new UserCreateDTO { UserName = "Volkner" });

            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        }
        [Test]
        public async Task CreateUser_ReturnsCreatedItem()
        {
            var result = await controller.PostUser(new UserCreateDTO { UserName = "Cheren" });

            var typedResult = result.Result as CreatedAtActionResult;
            var value = typedResult!.Value as User;

            // UserName is correct
            Assert.That(value!.UserName, Is.EqualTo("Cheren"));
            // Pokemon List is initialised as empty
            Assert.IsAssignableFrom<List<Pokemon>>(value!.Pokemon);
            Assert.That(value.Pokemon!, Is.Empty);
            // UserId is some integer
            Assert.That(value!.UserId, Is.InstanceOf<int>());

        }
    }
}