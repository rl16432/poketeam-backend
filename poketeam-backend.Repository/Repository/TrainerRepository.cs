﻿using Microsoft.EntityFrameworkCore;
using poketeam_backend.Domain.Models;
using poketeam_backend.Repository.Caching;
using poketeam_backend.Repository.Data;
using poketeam_backend.Repository.Extensions;
using poketeam_backend.Repository.IRepository;

namespace poketeam_backend.Repository.Repository
{
    public class TrainerRepository : BaseRepository<Trainer>, IUserRepository<Trainer>
    {
        public TrainerRepository(ApplicationDbContext appContext, ICacheService? distributedCache = null) : base(appContext, distributedCache)
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
        public override async Task<Trainer> GetAsync(int userId)
        {
            if (_cacheService == null)
            {
                return Get(userId);
            }

            return (await _appContext.Set<Trainer>().IncludeMultiple(user => user.Pokemon)
                .SingleOrDefaultAsync(u => u.Id == userId))!;
        }
        public async Task<Trainer> GetByUserNameAsync(string userName)
        {
            return (await entities.IncludeMultiple(user => user.Pokemon)
                .SingleOrDefaultAsync(user => user.UserName == userName))!;
        }
        public override async Task<IEnumerable<Trainer>> GetAllAsync()
        {
            if (_cacheService == null)
            {
                return GetAll();
            }

            IEnumerable<Trainer>? cachedData = _cacheService.TryGet<IEnumerable<Trainer>>(cacheKey);
            if (cachedData == null)
            {
                cachedData = await _appContext.Set<Trainer>().IncludeMultiple(user => user.Pokemon).ToListAsync();
                _cacheService.Set(cacheKey, cachedData);
            }
            return cachedData;
        }

        public override async Task RefreshCache()
        {
            if (_cacheService == null)
            {
                return;
            }
            _cacheService.Remove(cacheKey);
            var cachedList = await _appContext.Set<Trainer>().IncludeMultiple(u => u.Pokemon)
                .ToListAsync();
            _cacheService.Set(cacheKey, cachedList);
        }
    }
}