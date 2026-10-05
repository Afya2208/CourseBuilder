using System.Linq.Expressions;

namespace API.Util
{
    public interface IBaseRepository<TEntity> where TEntity : class, new()
    {
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity?> DeleteAsync(object id);
        Task<TEntity?> FindByIdAsync(object id, System.Linq.Expressions.Expression<Func<TEntity, object?>>[]? includes = null);
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, object>>[]? includes = null);
        Task<TEntity?> FindByConditionAsync(Expression<Func<TEntity, bool>> condition, 
            Expression<Func<TEntity, object>>[]? includes = null);
        Task<List<TEntity>> FindAllByConditionAsync(Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, object>>[]? includes = null);
    }
}