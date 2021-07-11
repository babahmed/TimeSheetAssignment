
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HimamaTimesheet.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        Task<long> AddAsync(T entity);
        Task<long> AddRangeAsync(List<T> entities);
        Task<long> CountAsync(Expression<Func<T, bool>> filter);
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(long entityId);
        Task<List<T>> GetListAsync();
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateRangeAsync(List<T> entities);
        Task<int> SqlQuery(string query);
    }
}
