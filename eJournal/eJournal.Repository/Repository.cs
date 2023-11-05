using eJournal.Domain.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eJournal.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly EJournalDbContext _context;
        private readonly DbSet<T> _entities;

        public Repository(EJournalDbContext eJournalDbContext)
        {
            _context = eJournalDbContext;
            _entities = _context.Set<T>();
        }
        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _entities.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while saving the data in database: " + ex);
            }
        }

        public async Task DeleteAsync(long id)
        {
            var result = await GetByIdAsync(id);

            if (result != null)
            {
                _entities.Remove(result);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No Data Found to Delete with id = " + id);
            }
        }

        public IAsyncEnumerable<T> GeneralSearch(Expression<Func<T, bool>>? predicate)
        {
            var result = _entities.Where(predicate).AsAsyncEnumerable<T>();
            return result;
        }

        public IAsyncEnumerable<T> GeneralSearch(Expression<Func<T, bool>> predicate, Expression<Func<T, DateTime>>? query, int skip = 0, int take = 0)
        {
            var result = _entities.Where(predicate).OrderByDescending(query).Skip(skip).Take(take).AsAsyncEnumerable();
            return result;
        }

        public async Task<IAsyncEnumerable<T>> GetAllAsync(Expression<Func<T, DateTime>>? query, int skip = 0, int take = 0)
        {
            // todo: take and skip
            IAsyncEnumerable<T> result;
            if (skip == 0 && take == 0)
            {
                result = _entities.AsAsyncEnumerable();
            }
            else
            {
                result = _entities.OrderByDescending(query).Skip(skip).Take(take).AsAsyncEnumerable();
            }
            return result;
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<IAsyncEnumerable<T>> GetAllAsync()
        {
            return (IAsyncEnumerable<T>)_entities.AsAsyncEnumerable();

        }
    }
}
