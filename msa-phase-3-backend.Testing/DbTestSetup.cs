using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Data;
using System.Text.Json;

namespace msa_phase_3_backend.Testing
{
    internal class DbTestSetup
    {
        public readonly List<string> UserNames = new() { "Bianca", "Ralph", "Skyla", "Diamond" };
        private readonly string userNameWithPokemon = "Diamond";
        public ApplicationDbContext AppContext { get; set; }
        public List<Pokemon> AddedPokemonList { get; set; }
        public List<Pokemon> NotAddedPokemonList { get; set; }
        public Trainer UserWithPokemon { get; set; }

        public DbTestSetup()
        {
            // Use EF Core in memory database for testing. New database on every test
            // Guid is virtually unique
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            AppContext = new ApplicationDbContext(options);

            foreach (string user in UserNames)
            {
                AppContext.Trainers.Add(new Trainer { UserName = user });
            }

            AppContext.SaveChanges();

            NotAddedPokemonList = GetPokemonListFromJson("pokemon_list.json");
            AddedPokemonList = new List<Pokemon>();

            // User which we add Pokemon to for testing purposes
            UserWithPokemon = AppContext.Trainers.FirstOrDefault(x => x.UserName.Equals(userNameWithPokemon))!;

            // Add first 3 of the list to "Diamond" user for testing purposes
            foreach (Pokemon pokemon in NotAddedPokemonList.GetRange(0, 3))
            {
                AppContext.Pokemon.Add(pokemon);
                UserWithPokemon!.Pokemon.Add(pokemon);
                AppContext.Trainers.Update(UserWithPokemon);

                AddedPokemonList.Add(pokemon);
                NotAddedPokemonList.Remove(pokemon);
            }

            AppContext.SaveChanges();
        }

        public static List<Pokemon> GetPokemonListFromJson(string filePath)
        {
            // Read test Pokemon list from JSON
            StreamReader r = new(Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestFiles", filePath));

            string json = r.ReadToEnd();

            return JsonSerializer.Deserialize<List<Pokemon>>(json)!;
        }
    }
}
