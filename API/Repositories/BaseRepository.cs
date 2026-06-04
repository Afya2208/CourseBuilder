
using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace API.Repositories
{
    public class BaseRepository<Entity>(DbContext context) where Entity : class
    {
        private DbSet<Entity> entities = context.Set<Entity>();
        public async Task<Entity> UpdateAsync(Entity entity)
        {
            Entity updatedEntity = entities.Update(entity).Entity;
            await context.SaveChangesAsync();
            return updatedEntity;
        }
        public async Task<Entity> AddAsync(Entity entity)
        {
            Entity addedEntity = (await entities.AddAsync(entity)).Entity;
            await context.SaveChangesAsync();
            return addedEntity;
        }
        public async Task<Entity?> DeleteAsync(object id)
        {
            Entity? entity = await entities.FindAsync(id);
            if (entity == null) return null;
            var deletedEntity = entities.Remove(entity).Entity;
            await context.SaveChangesAsync();
            return deletedEntity;
        }
        public async Task<Entity?> FindByIdAsync(object id, System.Linq.Expressions.Expression<Func<Entity, object?>>[]? includes = null)
        {
            Entity? entity = await entities.FindAsync(id);
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
        public async Task<List<Entity>> FindAllAsync(Expression<Func<Entity, object>>[]? includes = null)
        {
            IQueryable<Entity> query = entities;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return   await query.ToListAsync();
        }
        public async Task<Entity?> FindByConditionAsync(Expression<Func<Entity, bool>> condition, 
            Expression<Func<Entity, object>>[]? includes = null)
        {
            IQueryable<Entity> query = entities;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(condition);
        }   
        public async Task<List<Entity>> FindAllByConditionAsync(Expression<Func<Entity, bool>> condition,
            Expression<Func<Entity, object>>[]? includes = null)
        {
            IQueryable<Entity> query = entities;
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