using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Services.ICustomServices;
using msa_phase_3_backend.Repository.Repository;
using msa_phase_3_backend.Repository.IRepository;

namespace msa_phase_3_backend.Services.CustomServices
{
    public class TrainerServices : IUserCustomService<Trainer>
    {
        private readonly IUserRepository<Trainer> _trainerRepository;
        private readonly IRepository<Pokemon> _pokemonRepository;
        public TrainerServices(IUserRepository<Trainer> trainerRepository, IRepository<Pokemon> pokemonRepository)
        {
            _trainerRepository = trainerRepository;
            _pokemonRepository = pokemonRepository;
        }
        public void Delete(Trainer entity)
        {
            try
            {
                if (entity != null)
                {
                    _trainerRepository.Delete(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void DeleteById(int id)
        {
            try
            {
                var trainer = _trainerRepository.Get(id);
                if (trainer != null)
                {
                    _trainerRepository.Delete(trainer);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Trainer Get(int Id)
        {
            try
            {
                var obj = _trainerRepository.Get(Id);
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Trainer GetByUserName(string userName)
        {
            try
            {
                var obj = _trainerRepository.GetByUserName(userName);
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Trainer> GetAll()
        {
            try
            {
                var obj = _trainerRepository.GetAll();
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Insert(Trainer entity)
        {
            try
            {
                if (entity != null)
                {
                    _trainerRepository.Insert(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Remove(Trainer entity)
        {
            try
            {
                if (entity != null)
                {
                    _trainerRepository.Delete(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update(Trainer entity)
        {
            try
            {
                if (entity != null)
                {
                    _trainerRepository.Update(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}