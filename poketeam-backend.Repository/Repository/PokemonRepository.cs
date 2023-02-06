using poketeam_backend.Domain.Models;
using poketeam_backend.Repository.Data;

namespace poketeam_backend.Repository.Repository
{
    public class PokemonRepository : BaseRepository<Pokemon>
    {
        public PokemonRepository(ApplicationDbContext userContext) : base(userContext)
        {
        }
    }
}