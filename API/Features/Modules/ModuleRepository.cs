using API.Util;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Modules
{
    public interface IModuleRepository
    {
        Task<List<ModuleDto>> ReadAllCourseModulesAsync(int courseId);
        Task AddRangeAsync(List<Module> modules);
        Task<Module> AddAsync(Module module);
        Task SaveModulesOrderAsync(IEnumerable<ModuleOrderDto> modulesOrder);
        Task DeleteAsync(long moduleId);
        Task<Module> UpdateAsync(long moduleId, Module module);
        Task<Module?> ReadByIdAsync(long moduleId);
    }
    
    public class ModuleRepository(CoursesDbContext context) : IModuleRepository
    {
        public async Task<List<ModuleDto>> ReadAllCourseModulesAsync(int courseId)
        {
            return await context.Modules
                .AsNoTracking()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x=>x.Order)
                .SelectDto()
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<Module> modules)
        {
            await context.Modules.AddRangeAsync(modules);
            await context.SaveChangesAsync();
        }

        public async Task<Module> AddAsync(Module entity)
        {
            var added = await context.Modules.AddAsync(entity);
            await context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<Module?> ReadByIdAsync(long moduleId)
        {
            return await context.Modules.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == moduleId);
        }

        public async Task DeleteAsync(long moduleId)
        {
            var module = await context.Modules.FindAsync(moduleId);
            if (module == null)
                throw new NotFoundException("Не найден модуль", moduleId);
            context.Modules.Remove(module);
            await context.SaveChangesAsync();
        }

        public async Task<Module> UpdateAsync(long moduleId, Module module)
        {
            var oldModule = await context.Modules.FindAsync(moduleId);
            if (oldModule == null)
                throw new NotFoundException("Не найден модуль", moduleId);
            context.Entry(oldModule).CurrentValues.SetValues(module);
            await context.SaveChangesAsync();
            return oldModule;
        }

        public async Task SaveModulesOrderAsync(IEnumerable<ModuleOrderDto> modulesOrder)
        {
            var dictOrders = modulesOrder.ToDictionary(o => o.Id);
            var oldModules = context.Modules.Where(x=> dictOrders.Keys.Contains(x.Id));
            foreach (var module in oldModules)
            {
                module.Order = dictOrders[module.Id].Order;   
            }
            await context.SaveChangesAsync();
        }
    }
}