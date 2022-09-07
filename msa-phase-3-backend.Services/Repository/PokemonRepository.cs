using msa_phase_3_backend.Domain.Data;
using msa_phase_3_backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Services.IRepository;

namespace msa_phase_3_backend.Services.Repository
{
    public class PokemonRepository : BaseRepository<Pokemon>
    {
        public PokemonRepository(UserContext userContext) : base(userContext)
        {
        }
    }
}