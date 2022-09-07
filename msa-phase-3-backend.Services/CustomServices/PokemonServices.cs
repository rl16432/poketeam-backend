using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Services.ICustomServices;
using msa_phase_3_backend.Services.Repository;

namespace msa_phase_3_backend.Services.CustomServices
{
    public class PokemonServices : ICustomService<Pokemon>
    {
        private readonly PokemonRepository _pokemonRepository;
        public PokemonServices(PokemonRepository pokemonRepository)
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
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
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
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
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
                    _pokemonRepository.Remove(entity);
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