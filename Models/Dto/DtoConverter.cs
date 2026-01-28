using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Models.Entities;

namespace Models.Dto
{
    public static class DtoConverter
    {
        public static UserDto ToDto(this User entity)
        {
            var dto = new UserDto()
            {
                Id = entity.Id,
                Email = entity.Email
            };
            if (entity.Role != null)
            {
                dto.Role = new RoleDto()
                {
                    Id = entity.RoleId,
                    Name = entity.Role.Name  
                };
            }
            if (entity.UserInformation != null)
            {
                var info = entity.UserInformation;
                dto.UserInformation = new UserInformationDto()
                {
                    FirstName = info.FirstName,
                    LastName = info.LastName,
                    Phone = info.Phone,
                    Position = info.Position,
                    MiddleName = info.MiddleName,
                    UserId = info.UserId
                };
            }
            return dto;
        }
        public static LessonDto ToDto(this Lesson entity)
        {
            var dto = new LessonDto()
            {
                Id = entity.Id,
                Description = entity.Description,
                ModuleId = entity.ModuleId,
                Order= entity.Order,
                LessonTypeId = entity.LessonTypeId,
                Name = entity.Name  
            };
            return dto;
        }
        public static TaskTypeDto ToDto(this TaskType entity)
        {
            var dto = new TaskTypeDto()
            {
                Id = entity.Id,
                Name = entity.Name  
            };
            return dto;
        }
        public static TaskDto ToDto(this Models.Entities.Task entity)
        {
            var dto = new TaskDto()
            {
                TaskType = entity.TaskType?.ToDto(),
                Id = entity.Id,
                LessonId = entity.LessonId,
                Question = entity.Question,

            };
            return dto;
        }
        public static ModuleDto ToDto(this Module entity)
        {
            var dto = new ModuleDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                CourseId = entity.CourseId,
                Description = entity.Description,
                Order = entity.Order,
                LessonsCount = entity.Lessons?.Count
            };
            return dto;
        }
        public static CourseDto ToDto(this Course entity)
        {
            var dto = new CourseDto()
            {
                Id = entity.Id,
                Description = entity.Description,
                Name = entity.Name,
                Price = entity.Price,
                ModulesHaveOrder = entity.ModulesHaveOrder,
                MinimalCompletionPercentage = entity.MinimalCompletionPercentage,
                ModulesCount = entity.Modules?.Count,
                LessonsCount = entity.Modules?.Sum(x=>x.Lessons?.Count),
                Themes = entity.Themes?.ToList().ConvertAll(x=>x.ToDto())
            };
            return dto;
        }
        public static ThemeDto ToDto(this Theme entity)
        {
            var dto = new ThemeDto()
            {
                Id = entity.Id,
                Name = entity.Name
            };
            return dto;
        }
        public static Theme ToEntity(this ThemeDto dto)
        {
            var entity = new Theme()
            {
                Id = dto.Id,
                Name = dto.Name,
            };
            return entity;
        }
    }
}