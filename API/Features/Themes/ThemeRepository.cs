using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Themes
{
    public interface IThemeRepository
    {
        Task<List<ThemeDto>> ReadAllAsync();
        Task DeleteAsync(int themeId);
        Task<Theme> AddAsync(Theme theme);
        Task<Theme> UpdateAsync(int themeId, Theme theme);
    }
    
    public class ThemeRepository(CoursesDbContext context) : IThemeRepository
    {
        public async Task<List<ThemeDto>> ReadAllAsync()
        {
            return await context.Themes.Select(x=>new ThemeDto(x.Id, x.Name)).ToListAsync();
        }

        public async Task DeleteAsync(int themeId)
        {
            var themeToDelete = await FindThemeByIdAsync(themeId);
            if (themeToDelete == null)
                throw new NotFoundException("Ошибка операции удаления", themeId);
            context.Themes.Remove(themeToDelete);
            await context.SaveChangesAsync();
        }
        
        public async Task<Theme?> FindThemeByIdAsync(int themeId)
        {
            return await context.Themes.FirstOrDefaultAsync(x => x.Id == themeId);
        }

        public async Task<Theme> AddAsync(Theme theme)
        {
            var added = await context.Themes.AddAsync(theme);
            var saved = added.Entity;
            await context.SaveChangesAsync();
            return saved;
        }

        public async Task<Theme> UpdateAsync(int themeId, Theme themeDto)
        {
            var themeToUpdate = await FindThemeByIdAsync(themeId);
            if (themeToUpdate == null)
                throw new NotFoundException("Ошибка операции обновления", themeId);
            context.Entry(themeToUpdate).CurrentValues.SetValues(themeDto);
            await context.SaveChangesAsync();
            return themeToUpdate;
        }
    }
}