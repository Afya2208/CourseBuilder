using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Entities;

namespace Models.Dto
{
    public static class DtoConverter
    {
        public static ThemeDto ToDto(this Theme entity)
        {
            var dto = new ThemeDto()
            {
                Id = entity.Id,
                Name = entity.Name,
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