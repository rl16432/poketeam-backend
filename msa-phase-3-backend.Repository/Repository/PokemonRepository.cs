using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Data;

namespace msa_phase_3_backend.Repository.Repository
{
    public class PokemonRepository : BaseRepository<Pokemon>
    {
        public PokemonRepository(ApplicationDbContext userContext) : base(userContext)
        {
        }
    }
}