using System.Linq.Expressions;

namespace Connect.DataAccess.Repository.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(bool noTracking = false, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties);
        Task<T> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> Query(bool noTracking = false, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetPagedAsync<TKey>(
            int page,
            int pageSize,
            Expression<Func<T, TKey>> orderBy,
            bool descending = false,
            Expression<Func<T, bool>> predicate = null,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> TakeAsync(
            int count,
            Expression<Func<T, bool>> predicate = null,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task BulkInsertAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task SoftDeleteAsync(T entity);
        Task SoftDeleteRangeAsync(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
