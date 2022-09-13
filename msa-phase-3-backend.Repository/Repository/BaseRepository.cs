using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Caching;
using msa_phase_3_backend.Repository.Data;
using msa_phase_3_backend.Repository.IRepository;

namespace msa_phase_3_backend.Repository.Repository
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly string cacheKey = $"{typeof(T)}";
        protected readonly ApplicationDbContext _appContext;
        protected DbSet<T> entities;
        protected ICacheService? _cacheService;

        public BaseRepository(ApplicationDbContext appContext, ICacheService? cacheService = null)
        {
            _appContext = appContext;
            entities = _appContext.Set<T>();
            _cacheService = cacheService;

        }
        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entities.Remove(entity);
            _appContext.SaveChanges();
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
            _appContext.SaveChanges();
        }
        public virtual void SaveChanges()
        {
            _appContext.SaveChanges();
        }
        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entities.Update(entity);
            _appContext.SaveChanges();
        }

        public virtual async Task<T> GetAsync(int id)
        {
            if (_cacheService == null)
            {
                return Get(id);
            }

            return (await _appContext.Set<T>().SingleOrDefaultAsync(u => u.Id == id))!;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            if (_cacheService == null)
            {
                return GetAll();
            }

            IEnumerable<T>? cachedData = _cacheService.TryGet<IEnumerable<T>>(cacheKey);
            if (cachedData == null)
            {
                cachedData = await _appContext.Set<T>().ToListAsync();
                _cacheService.Set(cacheKey, cachedData);
            }
            return cachedData;
        }

        public async Task InsertAsync(T entity)
        {
            if (_cacheService == null)
            {
                Insert(entity);

                return;
            }
            _appContext.Set<T>().Add(entity);

            await _appContext.SaveChangesAsync();
            await RefreshCache();

        }
        public async Task UpdateAsync(T entity)
        {
            if (_cacheService == null)
            {
                Update(entity);
                return;
            }
            _appContext.Entry(entity).State = EntityState.Modified;
            await _appContext.SaveChangesAsync();
            await RefreshCache();

        }
        public async Task DeleteAsync(T entity)
        {
            if (_cacheService == null)
            {
                Delete(entity);
                return;
            }
            _appContext.Set<T>().Remove(entity);
            await _appContext.SaveChangesAsync();
            await RefreshCache();
        }

        public async Task RefreshCache()
        {
            if (_cacheService == null)
            {
                return;
            }
            _cacheService.Remove(cacheKey);
            var cachedList = await _appContext.Set<T>().ToListAsync();
            _cacheService.Set(cacheKey, cachedList);
        }
    }
}