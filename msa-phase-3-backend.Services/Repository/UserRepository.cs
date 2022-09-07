using msa_phase_3_backend.Domain.Data;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Services.IRepository;

namespace msa_phase_3_backend.Services.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(UserContext userContext) : base(userContext)
        {
        }
        public override User Get(int userId)
        {
            return entities.IncludeMultiple(user => user.Pokemon!).SingleOrDefault(user => user.Id == userId)!;
        }
        public override IEnumerable<User> GetAll()
        {
            return entities.IncludeMultiple(user => user.Pokemon!).AsEnumerable();
        }

        public void DeleteById(int userId)
        {
            var user = entities.IncludeMultiple(user => user.Pokemon!).SingleOrDefault(user => user.Id == userId);
            entities.Remove(user!);
            _userContext.SaveChanges();
        }
    }
}