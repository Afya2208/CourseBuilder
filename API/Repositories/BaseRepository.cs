using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        public async Task<Entity?> FindByIdAsync(object id, Expression<Func<Entity, object>>[]? includes = null)
        {
            Entity? entity = await entities.FindAsync(id);
            if (entity == null) return null;
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    await context.Entry(entity).Reference(include).LoadAsync();
                }
            }
            return entity;
        }  
        public async Task<List<Entity>> FindAllAsync(Expression<Func<Entity, object>>[]? includes = null)
        {
            if (includes != null)
            {
                foreach(var include in includes)
                {
                    entities.Include(include);
                }
            }
            return await entities.ToListAsync();
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