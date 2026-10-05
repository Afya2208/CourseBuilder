using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Lessons;


public interface ILessonService
{
    Task<LessonDto> ReadByIdAsync(long lessonId);
    Task<bool> IsLessonDoneByUserAsync(long lessonId, long userId);
    Task<NextOrPrevLessonLink> GetPreviousLessonIdAsync(long lessonId);
    Task<NextOrPrevLessonLink> GetNextLessonIdAsync(long lessonId);
    Task<List<LessonDto>> ReadAllByModuleIdAsync(long moduleId);
    Task SaveLessonsOrderAsync(List<LessonOrderDto> lessonOrders);
    Task DeleteAsync(long lessonId);
    Task<LessonDto> UpdateAsync(long lessonId, LessonDto lessonToUpdate);
    Task<LessonDto> AddAsync(LessonDto lessonToAdd);
}

public class LessonService(ILessonRepository lessonRepository) : ILessonService
{
    public async Task<LessonDto> ReadByIdAsync(long lessonId)
    {
        var lesson = await lessonRepository.ReadByIdAsync(lessonId);
        if (lesson == null)
            throw new NotFoundException("Не найдено занятие", lessonId);
        return lesson;
    }

    public async Task<bool> IsLessonDoneByUserAsync(long lessonId, long userId)
    {
        return await lessonRepository.IsLessonDoneByUserAsync(lessonId, userId);
    }

    public async Task<NextOrPrevLessonLink> GetPreviousLessonIdAsync(long lessonId)
    {
        return await lessonRepository.GetPreviousLessonIdAsync(lessonId);
    }

    public async Task<NextOrPrevLessonLink> GetNextLessonIdAsync(long lessonId)
    {
        return await lessonRepository.GetNextLessonIdAsync(lessonId);
    }

    public async Task<List<LessonDto>> ReadAllByModuleIdAsync(long moduleId)
    {
        return await lessonRepository.ReadAllByModuleIdAsync(moduleId);
    }

    public async Task SaveLessonsOrderAsync(List<LessonOrderDto> lessonOrders)
    {
        await lessonRepository.SaveLessonsOrderAsync(lessonOrders);
    }

    public async Task DeleteAsync(long lessonId)
    {
        await lessonRepository.DeleteAsync(lessonId);
    }

    public async Task<LessonDto> UpdateAsync(long lessonId, LessonDto lessonToUpdate)
    {
        var lesson = lessonToUpdate.MapToEntity();
        if (lesson.Id != lessonId)
            throw new ArgumentException("Данные занятия не совпадают с переданным идентификатором");
        var saved = await lessonRepository.UpdateAsync(lessonId, lesson);
        return new LessonDto(saved);
    }

    public async Task<LessonDto> AddAsync(LessonDto lessonToAdd)
    {
        var lesson = lessonToAdd.MapToEntity();
        var saved = await lessonRepository.AddAsync(lesson);
        return new LessonDto(saved);
    }
}