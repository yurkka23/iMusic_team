using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace iMusic.DAL.Abstractions;

public interface IRepositoryAsync<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
            int skip = 0, int take = int.MaxValue);
    Task<TEntity> GetFirstOrDefaultAsync(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);
 
    Task<IEnumerable<TEntity>> GetFromSqlRowAsync(
        string sqlRaw,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int skip = 0,
        int take = int.MaxValue
    );
    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter);
    Task InsertAsync(TEntity entity);
    Task InsertRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entityToUpdate);
    void Delete(TEntity entityToDelete);
    void Delete(IEnumerable<TEntity> entityToDelete);
}
