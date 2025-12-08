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
        public static UserDto ToFullDto(this User entity)
        {
            var dto = new UserDto()
            {
                Id = entity.Id,
                Email = entity.Email,
            };
            dto.Role = new RoleDto()
            {
            };
            dto.UserInformation = new UserInformationDto();
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