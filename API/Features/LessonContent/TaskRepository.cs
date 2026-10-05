using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace API.Features.LessonContent
{
    public interface ITaskRepository
    {
        Task SaveTasksOrderAsync(IEnumerable<TaskOrderDto> taskOrders);
        Task<List<TaskDto>> ReadAllLessonTasksAsync(long lessonId);
        Task<TaskDto?> ReadTaskByIdAsync(long taskId);
        Task DeleteAsync(long taskId);
        Task<Domain.Entities.Task> UpdateAsync(long taskId, TaskAddUpdateRequest request);
        Task<Domain.Entities.Task> AddAsync(TaskAddUpdateRequest request);
    }
    
    public class TaskRepository(CoursesDbContext context) : ITaskRepository
    {
        public async Task SaveTasksOrderAsync(IEnumerable<TaskOrderDto> taskOrders)
        {
            var dictOrders = taskOrders.ToDictionary(o => o.Id);
            var oldTasks = context.Tasks.Where(x=> dictOrders.Keys.Contains(x.Id));
            foreach (var task in oldTasks)
            {
                task.Order = dictOrders[task.Id].Order;   
            }
            await context.SaveChangesAsync();
        }

        public async Task<List<TaskDto>> ReadAllLessonTasksAsync(long lessonId)
        {
            return await context.Tasks
                .AsNoTracking()
                .Where(t => t.LessonId == lessonId)
                .Select(t => new TaskDto(t.Id, t.Question, t.TaskTypeId, t.LessonId, t.Order, t.Score))
                .ToListAsync();
        }

        public async Task<TaskDto?> ReadTaskByIdAsync(long taskId)
        {
            return await context.Tasks.Where(t=>t.Id == taskId).Select(t=> new TaskDto(t.Id, t.Question, t.TaskTypeId, t.LessonId, t.Order, t.Score)).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(long taskId)
        {
            var task = await context.Tasks.FindAsync(taskId);
            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
        }

        public async Task<Domain.Entities.Task> UpdateAsync(long taskId, TaskAddUpdateRequest request)
        {
            var oldTask = await context.Tasks
                .Include(x=>x.Correlations)
                .Include(x=>x.TaskAnswers)
                .FirstOrDefaultAsync(x => x.Id == taskId);
            if (oldTask == null)
                throw new NotFoundException("Не найдена задача", taskId);
            context.Entry(oldTask).CurrentValues.SetValues(request);
            oldTask.TaskAnswers.Clear();
            oldTask.Correlations.Clear();
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
                        oldTask.TaskAnswers.Add(answer);
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
                            oldTask.Correlations.Add(cor);
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
                            oldTask.TaskAnswers.Add(cor);
                        });
                        break;
                    }
                case 6:
                    {
                        // skip
                        break;
                    }
            }
            await context.SaveChangesAsync();
            return oldTask;
        }
        
        public async Task<Domain.Entities.Task> AddAsync(TaskAddUpdateRequest request)
        {
            var newTask = new Domain.Entities.Task();
            context.Entry(newTask).CurrentValues.SetValues(request);
            newTask.TaskAnswers.Clear();
            newTask.Correlations.Clear();
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
            var saved = await context.Tasks.AddAsync(newTask);
            await context.SaveChangesAsync();
            return saved.Entity;
        }
    }
}