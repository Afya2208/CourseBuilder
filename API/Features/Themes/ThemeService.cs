namespace API.Features.Themes;

public interface IThemeService
{
    public Task<List<ThemeDto>> ReadAllAsync();
    public Task<ThemeDto> AddAsync(ThemeDto themeDto);
    public Task<ThemeDto> UpdateAsync(int themeId, ThemeDto themeDto);
    public Task DeleteAsync(int themeId);
}

public class ThemeService(IThemeRepository themeRepository) : IThemeService
{
    public async Task<List<ThemeDto>> ReadAllAsync()
    {
        return await themeRepository.ReadAllAsync();
    }
    
    public async Task<ThemeDto> AddAsync(ThemeDto themeDto)
    {
        var added = await themeRepository.AddAsync(themeDto.MapToEntity());
        return new ThemeDto(added.Id, added.Name);
    }

    public async Task<ThemeDto> UpdateAsync(int themeId, ThemeDto themeDto)
    {
        var entity = themeDto.MapToEntity();
        if (entity.Id != themeId)
            throw new ArgumentException("Данные темы не совпадают с переданным идентификатором");
        var saved = await themeRepository.UpdateAsync(themeId, entity);
        return new ThemeDto(saved.Id, saved.Name);
    }

    public async Task DeleteAsync(int themeId)
    {
        await themeRepository.DeleteAsync(themeId);
    }
}
