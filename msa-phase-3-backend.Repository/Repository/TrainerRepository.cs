
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Repository.Extensions;
using msa_phase_3_backend.Repository.IRepository;

namespace msa_phase_3_backend.Repository.Repository
{
    public class TrainerRepository : BaseRepository<Trainer>, IUserRepository<Trainer>
    {
        public TrainerRepository(ApplicationDbContext appContext) : base(appContext)
        {
        }
        public override Trainer Get(int userId)
        {
            return entities.IncludeMultiple(user => user.Pokemon).SingleOrDefault(user => user.Id == userId)!;
        }
        public Trainer GetByUserName(string userName)
        {
            return entities.IncludeMultiple(user => user.Pokemon).SingleOrDefault(user => user.UserName == userName)!;
        }
        public override IEnumerable<Trainer> GetAll()
        {
            return entities.IncludeMultiple(user => user.Pokemon).AsEnumerable();
        }
    }
}