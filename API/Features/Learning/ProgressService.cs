using API.Features.Courses;
using API.Features.Groups;
using API.Features.Lessons;
using API.Util;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Learning;


public interface IProgressService
{
    Task<GroupProgressDto> GetGroupProgressInCourseXlsxAsync(int groupId, int courseId);
    Task SaveUserTryLessonAsync(long userId, long lessonId, List<UserTrySolveDetail> details);
    Task AddAdditionalTryToUserAsync(long userId, long lessonId);
    Task<CourseProgressDto> GetCourseProgressForUserAsync(long userId, int courseId);
    Task<int?> GetUnusedTriesAsync(long userId, long lessonId);
    Task<bool> CheckIsLessonDoneByUserAsync(long userId, long lessonId);
}

public class ProgressService(IProgressRepository progressRepository, IGroupRepository groupRepository,
    ILessonRepository lessonRepository, ICourseRepository courseRepository) : IProgressService
{
    public async Task<GroupProgressDto> GetGroupProgressInCourseXlsxAsync(int groupId, int courseId)
    {
        var courseName = await courseRepository.ReadCourseNameByIdAsync(courseId);
        var groupName = await groupRepository.ReadGroupNameByIdAsync(groupId);
        
        if (string.IsNullOrWhiteSpace(courseName))
            throw new NotFoundException("Не найден курс", courseId);
        if (string.IsNullOrWhiteSpace(groupName))
            throw new NotFoundException("Не найдена группа", groupId);
        
        var usersInGroup = await groupRepository.ReadUsersInGroupAsync(groupId);
        var lessons = await lessonRepository.ReadCourseLessonsInfoForReportAsync(courseId);

        var usersIds = usersInGroup.Select(x => x.UserId);
        var lessonsIds = lessons.Select(x => x.Id);
        
        var tries = await progressRepository.ReadUsersTriesInLessonsForReportAsync(usersIds, lessonsIds);
        var fileXlsx = GetXlsxGroupProgressForCourse(groupName, courseName, usersInGroup, lessons, tries);
        return new GroupProgressDto(fileXlsx, groupName, courseName);
    }

    public async Task SaveUserTryLessonAsync(long userId, long lessonId, List<UserTrySolveDetail> details)
    {
        var result = await progressRepository.SaveUserTryLessonAsync(userId, lessonId, details);
        if (result.AllSolved)
        {
            var checkResult = await progressRepository.CheckIfCourseIsNeededToStateDoneByUserAsync(result.UserId, result.LessonId);
            if (checkResult.IsNeededToStateCompleted)
            {
                await progressRepository.SaveUserCompletedCourseAsync(result.UserId, checkResult.CourseId);
            }
        }
    }

    public async Task AddAdditionalTryToUserAsync(long userId, long lessonId)
    {
        await progressRepository.AddAdditionalTryToUserAsync(userId, lessonId);
    }

    public async Task<CourseProgressDto> GetCourseProgressForUserAsync(long userId, int courseId)
    {
        return await progressRepository.GetCourseProgressForUserAsync(userId, courseId);
    }

    public async Task<int?> GetUnusedTriesAsync(long userId, long lessonId)
    {
        return await progressRepository.GetUnusedTriesAsync(userId, lessonId);
    }

    public async Task<bool> CheckIsLessonDoneByUserAsync(long userId, long lessonId)
    {
        return await progressRepository.CheckIsLessonDoneByUserAsync(userId, lessonId);
    }

    public Stream GetXlsxGroupProgressForCourse(string groupName, string courseName, List<UserInGroupDto> students,
            List<LessonInfoForReportDto> lessons, List<UserTryInfoDto> userTries)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add();
           
            worksheet.Cell(1, 1).Value = $"Успеваемость группы {groupName} - курс {courseName}";
            worksheet.Range(1, 1, 1, 10).Merge();

            worksheet.Cell(5, 1).Value = $"Максимальный балл";

            var groupedLessons = lessons
            .OrderBy(x => x.ModuleOrder)
            .ThenBy(x => x.LessonOrder)
            .GroupBy(x => x.ModuleName)
            .ToList();

            int currentColumn = 2;

            foreach (var module in groupedLessons)
            {
                int moduleStartColumn = currentColumn;
                foreach (var lesson in module)
                {
                    worksheet.Cell(4, currentColumn).Value = lesson.Name;
                    worksheet.Cell(5, currentColumn).Value = lesson.MaxScore;
                    currentColumn++;
                }

                int moduleEndColumn = currentColumn - 1;
                worksheet.Range(3, moduleStartColumn, 3, moduleEndColumn).Merge();
                worksheet.Cell(3, moduleStartColumn).Value = module.Key;
                worksheet.Cell(3, moduleStartColumn).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;
            }

            int studentRow = 6;
            

            foreach (var student in students)
            {
                worksheet.Cell(studentRow, 1).Value =
                    $"{student.LastName} {student.FirstName}";

                int lessonColumn = 2;

                foreach (var lesson in lessons
                            .OrderBy(x => x.ModuleOrder)
                            .ThenBy(x => x.LessonOrder))
                {
                    var userTry = userTries.FirstOrDefault(x =>
                        x.UserId == student.UserId &&
                        x.LessonId == lesson.Id);

                    worksheet.Cell(studentRow, lessonColumn).Value =
                        userTry?.SumScore;

                    lessonColumn++;
                }

                studentRow++;
            }

            worksheet.Columns().AdjustToContents();
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream;
        }
}