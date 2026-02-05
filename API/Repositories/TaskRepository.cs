using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Repositories
{
    public class TaskRepository(CoursesDbContext context) : BaseRepository<Models.Entities.Task>(context)
    {
        public async System.Threading.Tasks.Task<Models.Entities.Task> UpdateAsync(TaskAddUpdateRequest request)
        {
            var oldTask = await context.Tasks
            .Include(x => x.Correlations)
            .Include(x => x.TaskAnswers)
            .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (oldTask == null) throw new NotFoundException("Задание не найдено");
            oldTask.Order = request.Order;
            oldTask.LessonId = request.LessonId;
            oldTask.TaskTypeId = request.TaskTypeId;
            oldTask.Question = request.Question ?? "";
            context.Correlations.RemoveRange(oldTask.Correlations);
            context.TaskAnswers.RemoveRange(oldTask.TaskAnswers);
            switch (request.TaskTypeId)
            {
                case 1:
                    
                        var answer = new TaskAnswer()
                        {
                            IsRight = true,
                            TextValue = request.TextAnswer,
                            TaskId = request.Id
                        };
                        oldTask.TaskAnswers.Add(answer);
                        await context.TaskAnswers.AddAsync(answer);
                        break;
                    
                case 2:
                    
                        // skip
                        break;
                    
                case 3:
                    
                        request.Correlations?.ForEach(async x =>
                        {
                            var cor = new Correlation()
                            {
                                Right = x.Right,
                                Left = x.Left,
                                TaskId = request.Id
                            };
                            oldTask.Correlations.Add(cor);
                            await context.Correlations.AddAsync(cor);
                        });
                        break;
                    
                case 4:
                case 5:
                        request.AllAnswerOptions?.ForEach(x =>
                        {
                            var ans = new TaskAnswer()
                            {
                                TextValue = x.TextValue,
                                IsRight = x.IsRight,
                                TaskId = request.Id
                            };
                            oldTask.TaskAnswers.Add(ans);
                            context.TaskAnswers.AddAsync(ans);
                        });
                        break;
                case 6:
                        break;
            }
            context.Tasks.Update(oldTask);
            await context.SaveChangesAsync();
            return oldTask;
        }

        public async System.Threading.Tasks.Task<Models.Entities.Task> AddAsync(TaskAddUpdateRequest request)
        {
            var newTask = new Models.Entities.Task()
            {
                Order = request.Order,
                LessonId = request.LessonId,
                TaskTypeId = request.TaskTypeId,
                Question = request.Question ?? "",
            };
            switch (request.TaskTypeId)
            {
                case 1:
                    {
                        var answer = new TaskAnswer()
                        {
                            IsRight = true,
                            TaskId = request.Id,
                            TextValue = request.TextAnswer
                        };
                        newTask.TaskAnswers.Add(answer);
                        break;
                    }
                case 2:
                    {
                        // skip
                        break;
                    }
                case 3:
                    {
                        request.Correlations?.ForEach(x =>
                        {
                            var cor = new Correlation()
                            {
                                Right = x.Right,
                                Left = x.Left,
                                TaskId = request.Id
                            };
                            newTask.Correlations.Add(cor);
                        });
                        break;
                    }
                case 4:
                case 5:
                    {
                        request.AllAnswerOptions?.ForEach(x =>
                        {
                            var cor = new TaskAnswer()
                            {
                                TextValue = x.TextValue,
                                IsRight = x.IsRight ?? false,
                                TaskId = request.Id
                            };
                            newTask.TaskAnswers.Add(cor);
                        });
                        break;
                    }
                case 6:
                    {
                        // skip
                        break;
                    }
            }
            await context.Tasks.AddAsync(newTask);
            await context.SaveChangesAsync();
            return newTask;
        }
    }
}