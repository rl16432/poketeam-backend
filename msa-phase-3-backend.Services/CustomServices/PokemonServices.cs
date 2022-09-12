using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Services.ICustomServices;
using msa_phase_3_backend.Repository.IRepository;

namespace msa_phase_3_backend.Services.CustomServices
{
    public class PokemonServices : ICustomService<Pokemon>
    {
        private readonly IRepository<Pokemon> _pokemonRepository;
        public PokemonServices(IRepository<Pokemon> pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;
        }
        public void Delete(Pokemon entity)
        {
            try
            {
                if (entity != null)
                {
                    _pokemonRepository.Delete(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Pokemon Get(int Id)
        {
            try
            {
                var obj = _pokemonRepository.Get(Id);
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Pokemon> GetAll()
        {
            try
            {
                var obj = _pokemonRepository.GetAll();
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Insert(Pokemon entity)
        {
            try
            {
                if (entity != null)
                {
                    _pokemonRepository.Insert(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Remove(Pokemon entity)
        {
            try
            {
                if (entity != null)
                {
                    _pokemonRepository.Delete(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update(Pokemon entity)
        {
            try
            {
                if (entity != null)
                {
                    _pokemonRepository.Update(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}