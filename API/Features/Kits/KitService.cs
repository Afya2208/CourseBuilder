using API.Features.Courses;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Kits;

public interface IKitService
{
    Task<List<KitDto>> ReadAllKitsByUserIdAsync(long userId);
    Task AddKitToUserAsync(long userId, int kitId);
    Task<List<KitShortDto>> ReadAllKitsUserHasAsync(long userId);
    Task<List<int>> ReadAllKitsIdUserHasAsync(long userId);
}

public class KitService(IKitRepository kitRepository) : IKitService
{
    public Task<KitsDtoList> PagingSearchKitsAsync(int pageSize, int pageNumber, string? text, int? themeId)
    {
        throw new NotImplementedException();
    }

    public Task<KitDto> AddKitAsync(KitDto kitDto)
    {
        throw new NotImplementedException();
    }

    public Task<KitDto> UpdateKitAsync(int kitId, KitDto kitDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteKitAsync(int kitId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<KitDto>> ReadAllKitsByUserIdAsync(long userId)
    {
        return await kitRepository.ReadAllKitsByUserIdAsync(userId);
    }

    public async Task AddKitToUserAsync(long userId, int kitId)
    {
        await kitRepository.AddKitToUserAsync(userId, kitId);
    }

    public async Task<List<KitShortDto>> ReadAllKitsUserHasAsync(long userId)
    {
        return await kitRepository.ReadAllKitsUserHasAsync(userId);
    }

    public async Task<List<int>> ReadAllKitsIdUserHasAsync(long userId)
    {
        return await kitRepository.ReadAllKitsIdUserHasAsync(userId);
    }

    
}