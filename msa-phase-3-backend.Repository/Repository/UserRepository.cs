
using msa_phase_3_backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Repository.Extensions;

namespace msa_phase_3_backend.Repository.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(UserContext userContext) : base(userContext)
        {
        }
        public override User Get(int userId)
        {
            return entities.IncludeMultiple(user => user.Pokemon!).SingleOrDefault(user => user.userId == userId)!;
        }
        public User GetByUserName(string userName)
        {
            return entities.IncludeMultiple(user => user.Pokemon!).SingleOrDefault(user => user.UserName == userName)!;
        }
        public override IEnumerable<User> GetAll()
        {
            return entities.IncludeMultiple(user => user.Pokemon!).AsEnumerable();
        }

        public void DeleteById(int userId)
        {
            var user = entities.IncludeMultiple(user => user.Pokemon!).SingleOrDefault(user => user.userId == userId);
            entities.Remove(user!);
            _userContext.SaveChanges();
        }
    }
}