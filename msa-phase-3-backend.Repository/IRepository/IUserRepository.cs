using msa_phase_3_backend.Domain.Models;

namespace msa_phase_3_backend.Repository.IRepository
{
    public interface IUserRepository<T> : IRepository<T> where T : class
    {
        T GetByUserName(string userName);
    }
}