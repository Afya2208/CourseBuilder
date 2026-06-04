using System;
using System.Collections.Generic;
using System.Linq;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class TaskController(TaskRepository taskRepository, TaskTypeRepository taskTypeRepository,
        CoursesDbContext context) : ControllerBase
    {
        [HttpGet("tasks/{taskId:long}")]
        public async Task<IActionResult> FindById(long taskId)
        {
            var task = await context.Tasks.FindAsync(taskId);
            if (task == null) throw new NotFoundException($"Не найдена задача id={taskId}");
            return Ok(task);
        }
        [HttpGet("task-types")]
        public async Task<IActionResult> FindAllTypes()
        {
            var types = await taskTypeRepository.FindAllAsync();
            var typesDto = types.ConvertAll(x=>x.ToDto());
            return Ok(typesDto);
        }

        [HttpGet("lessons/{lessonId:long}/tasks")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {
            var tasks = await context.Tasks
                .Where(x => x.LessonId == lessonId)
                .Select(x=> new
                {
                    x.Id, x.LessonId, x.TaskTypeId, x.Order,
                    x.Question, x.Score
                })
                .OrderBy(x=>x.Order)
                .ToListAsync();
            return Ok(tasks);
        }
        
        [HttpPost("/tasks/order")]
        public async Task<IActionResult> SaveLessonsOrder([FromBody] List<LessonOrder> taskOrders)
        {
            foreach (var taskOrder in taskOrders)
            {
                var task = await context.Tasks.FindAsync(taskOrder.Id);
                task.Order = taskOrder.Order;
            }
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("tasks/{taskId:long}")]
        public async Task<IActionResult> Delete(long taskId)
        {
            return Ok(await taskRepository.DeleteAsync(taskId));
        }
        [HttpPost("tasks")]
        //[Authorize(Roles = "Разработчик")]
        public async Task<IActionResult> Add([FromBody] TaskAddUpdateRequest request)
        {
            var newTask = new Models.Entities.Task();
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
            await context.Tasks.AddAsync(newTask);
            await context.SaveChangesAsync();
            return Ok(newTask);
        }
        
        [HttpPut("tasks")]
        public async Task<IActionResult> Update([FromBody] TaskAddUpdateRequest request)
        {
            var oldTask = await context.Tasks
                .Include(x=>x.Correlations)
                .Include(x=>x.TaskAnswers)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
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
            return Ok(oldTask);
        }
    }
}