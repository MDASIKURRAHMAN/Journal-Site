using System.Linq.Expressions;

namespace eJournal.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(long id);
        Task<T> GetByIdAsync(long id);
        Task<IAsyncEnumerable<T>> GetAllAsync(Expression<Func<T, DateTime>> query, int skip = 0, int take = 0);
        IAsyncEnumerable<T> GeneralSearch(Expression<Func<T, bool>> predicate);
        IAsyncEnumerable<T> GeneralSearch(Expression<Func<T, bool>> predicate, Expression<Func<T, DateTime>>? query, int skip = 0, int take = 0);
        Task<IAsyncEnumerable<T>> GetAllAsync();
    }
}
