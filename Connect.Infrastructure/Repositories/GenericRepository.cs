using System.Linq.Expressions;
using Connect.Domain.Entities;
using Connect.Infrastructure.Data;
using Connect.Infrastructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Connect.Infrastructure.Repository
{

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id, bool noTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool noTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<T> Query(bool noTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            return query;
        }

        public async Task<IEnumerable<T>> GetPagedAsync<TKey>(
            int page,
            int pageSize,
            Expression<Func<T, TKey>> orderBy,
            bool descending = false,
            Expression<Func<T, bool>> predicate = null,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than 0.", nameof(page));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than 0.", nameof(pageSize));

            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<T>> TakeAsync(
            int count,
            Expression<Func<T, bool>> predicate = null,
            bool noTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            if (count < 0) throw new ArgumentException("Count must be non-negative.", nameof(count));

            var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.Take(count).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {
            // Note: For actual bulk insert, you may need EFCore.BulkExtensions or similar
            // This is a fallback implementation using AddRangeAsync
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            _context.SaveChanges();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            _context.SaveChanges();
        }

        public async Task SoftDeleteAsync(T entity)
        {
            if (entity is Post post)
            {
                post.IsDeleted = true;
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Entity does not implement ISoftDeletable.");
            }
        }
            
        public async Task SoftDeleteRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable softDeletable)
                {
                    softDeletable.IsDeleted = true;
                }
                else
                {
                    throw new InvalidOperationException("One or more entities do not implement ISoftDeletable.");
                }
            }
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                var query = _dbSet.AsQueryable().Where(e => !((ISoftDeletable)e).IsDeleted);
                return predicate == null
                    ? await query.CountAsync()
                    : await query.Where(predicate).CountAsync();
            }
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.Where(predicate).CountAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

    }
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
