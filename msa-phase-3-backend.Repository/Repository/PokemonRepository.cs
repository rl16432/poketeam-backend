using msa_phase_3_backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Repository.IRepository;
using msa_phase_3_backend.Repository.Data;

namespace msa_phase_3_backend.Repository.Repository
{
    public class PokemonRepository : BaseRepository<Pokemon>
    {
        public PokemonRepository(UserContext userContext) : base(userContext)
        {
        }
    }
}