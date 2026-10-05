using Domain.Exceptions;

namespace API.Features.Courses;

public interface ICourseService
{
    Task<CourseDto> ReadByIdAsync(int courseId);
    Task<CoursesDtoList> PagingSearchAsync(int pageSize, int pageNumber, string? text, int? themeId);
    Task ChangePublicityAsync(int courseId);
    Task<CourseDto> AddAsync(AddUpdateCourseRequest request);
    Task<CourseDto> UpdateAsync(int courseId, AddUpdateCourseRequest request);
    Task DeleteAsync(int courseId);
    Task<List<CourseDto>> ReadAllCoursesByUserIdAsync(long userId);
    Task<List<CourseShortDto>> ReadAllCoursesShortByUserIdAsync(long userId);
    Task<List<CourseDto>> ReadAllCoursesUserHasAsync(long userId);
    Task AddCourseToUserAsync(long userId, int courseId);
    Task<List<int>> ReadAllCoursesIdUserHasAsync(long userId);
    Task<List<CourseShortDto>> ReadAllCoursesShortUserHasAsync(long userId);
    Task<bool> CheckIsUserCourseAuthorAsync(long userId, int courseId);
}


public class CourseService(ICourseRepository courseRepository) : ICourseService
{
    public async Task<CourseDto> ReadByIdAsync(int courseId)
    {
        var course = await courseRepository.ReadByIdAsync(courseId);
        if (course == null)
            throw new NotFoundException("Не найден курс", courseId);
        return course;
    }

    public async Task<CoursesDtoList> PagingSearchAsync(int pageSize, int pageNumber, string? text, int? themeId)
    {
        return await courseRepository.PagingSearchAsync(pageSize, pageNumber, text, themeId);
    }

    public async Task ChangePublicityAsync(int courseId)
    {
        await courseRepository.ChangePublicityAsync(courseId);
    }

    public async Task<CourseDto> AddAsync(AddUpdateCourseRequest request)
    {
        var courseToAdd = request.MapToEntity();
        var saved = await courseRepository.AddAsync(courseToAdd, request.ThemesIds ?? []);
        return new CourseDto(saved);
    }

    public async Task<CourseDto> UpdateAsync(int courseId, AddUpdateCourseRequest request)
    {
        var courseData = request.MapToEntity();
        if (courseData.Id != courseId)
            throw new ArgumentException("Данные курса не совпадают по переданному идентификатору");
        var saved = await courseRepository.UpdateAsync(courseId, courseData, request.ThemesIds ?? []);
        return new CourseDto(saved);
    }

    public async Task DeleteAsync(int courseId)
    {
        await courseRepository.DeleteAsync(courseId);
    }

    public async Task<List<CourseDto>> ReadAllCoursesByUserIdAsync(long userId)
    {
        return await courseRepository.ReadAllCoursesByUserIdAsync(userId);
    }

    public async Task<List<CourseShortDto>> ReadAllCoursesShortByUserIdAsync(long userId)
    {
        return await courseRepository.ReadAllCoursesShortByUserIdAsync(userId);
    }

    public async Task<List<CourseDto>> ReadAllCoursesUserHasAsync(long userId)
    {
        return await courseRepository.ReadAllCoursesUserHasAsync(userId);
    }

    public async Task AddCourseToUserAsync(long userId, int courseId)
    {
        await courseRepository.AddCourseToUserAsync(userId, courseId);
    }

    public async Task<List<int>> ReadAllCoursesIdUserHasAsync(long userId)
    {
        return await courseRepository.ReadAllCoursesIdUserHasAsync(userId);
    }

    public async Task<List<CourseShortDto>> ReadAllCoursesShortUserHasAsync(long userId)
    {
        return await courseRepository.ReadAllCoursesShortUserHasAsync(userId);
    }

    public async Task<bool> CheckIsUserCourseAuthorAsync(long userId, int courseId)
    {
        return await courseRepository.CheckIsUserCourseAuthorAsync(userId, courseId);
    }
}