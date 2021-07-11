using AspNetCoreHero.Abstractions.Domain;
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Application.Interfaces.Shared;
using HimamaTimesheet.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PublicWorkflow.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AuditableEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthenticatedUserService _user;
        private IQueryable<T> Entity;

        public GenericRepository(ApplicationDbContext dbContext, IAuthenticatedUserService user)
        {
            _dbContext = dbContext;
            _user = user;

            Entity = _dbContext.Set<T>();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            var entity = Entity;

            IQueryable<T> query = entity;

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query,
                            (current, include) => current.Include(include));
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            var entity = Entity;

            if (includeProperties != null)
            {
                entity = includeProperties.Aggregate(entity,
                            (current, include) => current.Include(include));
            }

            return await Task.FromResult(entity.Where(filter).AsQueryable<T>());
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includeProperties)
        {
            var entity = Entity;

            if (includeProperties != null)
            {
                entity = includeProperties.Aggregate(entity,
                            (current, include) => current.Include(include));
            }

            var data = entity.Where(filter).AsQueryable<T>();
            data = data
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .AsQueryable();

            return await Task.FromResult(data);
        }

        public async Task<T> GetByIdAsync(long entityId)
        {
            var entity = Entity;

            return await entity.FirstOrDefaultAsync(p => p.Id == entityId);
        }

        public async Task<List<T>> GetListAsync()
        {
            var entity = Entity;

            return await entity.IgnoreQueryFilters().ToListAsync();
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> filter)
        {
            var entity = Entity;

            return await entity.CountAsync(filter);
        }

        public async Task<long> AddAsync(T entity)
        {
            entity.CreatedBy = _user?.Username;
            entity.CreatedOn = DateTime.Now;
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<long> AddRangeAsync(List<T> entities)
        {
            entities.ForEach(a =>
            {
                a.CreatedBy = _user?.Username;
                a.CreatedOn = DateTime.Now;
            });
            await _dbContext.AddRangeAsync(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            entity.LastModifiedBy = _user?.Username;
            entity.LastModifiedOn = DateTime.Now;
            _dbContext.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRangeAsync(List<T> entities)
        {
            entities.ForEach(a =>
            {
                a.LastModifiedBy = _user?.Username;
                a.LastModifiedOn = DateTime.Now;
            });
            _dbContext.UpdateRange(entities);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> SqlQuery(string query)
        {
            if (query.ToLower().StartsWith("drop") || query.ToLower().StartsWith("alter")
                || query.ToLower().StartsWith("truncate") || query.ToLower().StartsWith("create"))
                throw new ArgumentException("DDL commands not allowed");

            return await _dbContext.Database.ExecuteSqlRawAsync(query);
        }
    }
}

