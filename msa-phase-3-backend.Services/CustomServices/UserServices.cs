using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Services.ICustomServices;
using msa_phase_3_backend.Repository.Repository;

namespace msa_phase_3_backend.Services.CustomServices
{
    public class UserServices : ICustomService<User>
    {
        private readonly UserRepository _userRepository;
        private readonly PokemonRepository _pokemonRepository;
        public UserServices(UserRepository userRepository, PokemonRepository pokemonRepository)
        {
            _userRepository = userRepository;
            _pokemonRepository = pokemonRepository;
        }
        public void Delete(User entity)
        {
            try
            {
                if (entity != null)
                {
                    _userRepository.Delete(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a user and all its Pokemon
        /// </summary>
        /// <param name="userId">The ID of the user to delete</param>
        public void DeleteById(int userId)
        {
            try
            {
                var user = _userRepository.Get(userId);
                if (user == null)
                {
                    return;
                }
                if (user != null && user.Pokemon != null)
                {
                    foreach (Pokemon pokemon in user.Pokemon)
                    {
                        _pokemonRepository.Delete(pokemon);
                    }
                    _userRepository.Delete(user);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public User Get(int Id)
        {
            try
            {
                var obj = _userRepository.Get(Id);
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
        public IEnumerable<User> GetAll()
        {
            try
            {
                var obj = _userRepository.GetAll();
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
        public void Insert(User entity)
        {
            try
            {
                if (entity != null)
                {
                    _userRepository.Insert(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Remove(User entity)
        {
            try
            {
                if (entity != null)
                {
                    _userRepository.Remove(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update(User entity)
        {
            try
            {
                if (entity != null)
                {
                    _userRepository.Update(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}