using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Models.Dto;
using Models.Entities;

namespace API.Service
{
    public class ProgressDocumentor
    {
        public static async Task<byte[]> XlsxGroupProgressForCourse(string groupName, string courseName, List<InfoAboutUser> students,
            List<LessonInfo> lessons, List<UserTriesInfo> userTries)
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
                    // Строка с названием лекции
                    worksheet.Cell(4, currentColumn).Value = lesson.Name;

                    // Строка с максимальным баллом
                    worksheet.Cell(5, currentColumn).Value = lesson.MaxScore;

                    currentColumn++;
                }

                int moduleEndColumn = currentColumn - 1;

                // Объединяем ячейки под название модуля
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
                    // Ищем попытку/балл
                    var userTry = userTries.FirstOrDefault(x =>
                        x.UserId == student.Id &&
                        x.LessonId == lesson.Id);

                    worksheet.Cell(studentRow, lessonColumn).Value =
                        userTry?.SumScore;

                    lessonColumn++;
                }

                studentRow++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}