using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Repository.IRepository;

namespace msa_phase_3_backend.Repository.Repository
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _userContext;
        protected DbSet<T> entities;
        public BaseRepository(ApplicationDbContext userContext)
        {
            _userContext = userContext;
            entities = _userContext.Set<T>();
        }
        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entities.Remove(entity);
            _userContext.SaveChanges();
        }
        public virtual T Get(int Id)
        {
            return entities.SingleOrDefault(c => c.Id == Id)!;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }
        public virtual void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entities.Add(entity);
            _userContext.SaveChanges();
        }
        public virtual void SaveChanges()
        {
            _userContext.SaveChanges();
        }
        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entities.Update(entity);
            _userContext.SaveChanges();
        }
    }
}