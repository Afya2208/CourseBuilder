using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace API.Util
{
    public class BaseRepository<TEntity>(DbContext context) : IBaseRepository<TEntity> where TEntity : class, new()
    {
        protected DbSet<TEntity> entities = context.Set<TEntity>();
        
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            TEntity updatedEntity = entities.Update(entity).Entity;
            await context.SaveChangesAsync();
            return updatedEntity;
        }
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            TEntity addedEntity = (await entities.AddAsync(entity)).Entity;
            await context.SaveChangesAsync();
            return addedEntity;
        }
        public virtual async Task<TEntity?> DeleteAsync(object id)
        {
            TEntity? entity = await entities.FindAsync(id);
            if (entity == null) return null;
            var deletedEntity = entities.Remove(entity).Entity;
            await context.SaveChangesAsync();
            return deletedEntity;
        }
        public virtual async Task<TEntity?> FindByIdAsync(object id, System.Linq.Expressions.Expression<Func<TEntity, object?>>[]? includes = null)
        {
            TEntity? entity = await entities.FindAsync(id);
            if (entity == null) return null;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    var s =include.Body.ToString().Split(".")[1];
                    if (typeof(IEnumerable).IsAssignableFrom(include.Body.Type))
                    {
                        await context.Entry(entity).Navigation(s).LoadAsync();
                    }
                   else
                    {
                        await context.Entry(entity).Reference(include).LoadAsync();
                    }
                    
                }
            }
            return entity;
        }  
        public virtual async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, object>>[]? includes = null)
        { 
            IQueryable<TEntity> query = entities;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return   await query.ToListAsync();
        }
        public virtual async Task<TEntity?> FindByConditionAsync(Expression<Func<TEntity, bool>> condition, 
            Expression<Func<TEntity, object>>[]? includes = null)
        {
            IQueryable<TEntity> query = entities;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(condition);
        }   
        public virtual async Task<List<TEntity>> FindAllByConditionAsync(Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, object>>[]? includes = null)
        {
            IQueryable<TEntity> query = entities;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(condition).ToListAsync();
        }
    }

}